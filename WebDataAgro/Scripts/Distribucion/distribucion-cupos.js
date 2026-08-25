const CLASS_META = {
  'MP-Fason': { label: 'MP-Fasón', cls: 'p1' },
  'MP-Prest/Devolución': { label: 'MP-Préstamo/Dev.', cls: 'p2' },
  'Fijo': { label: 'Fijo', cls: 'p3' },
  'Contrato Hijo': { label: 'Fijo (C.Hijo)', cls: 'p3' },
  'A Fijar': { label: 'A Fijar', cls: 'p4' }
};
const CLASS_ORDER = ['MP-Fason', 'MP-Prest/Devolución', 'Fijo', 'Contrato Hijo', 'A Fijar'];
const OP_TYPES = [
  { id: 'con_corredor', label: 'Con corredor', colorClass: 'col-cc' },
  { id: 'acopiador', label: 'Acopiador directo', colorClass: 'col-ac' },
  { id: 'productor', label: 'Productor directo', colorClass: 'col-pr' },
  { id: 'otros', label: 'Otros / Sin clasif.', colorClass: 'col-ot' }
];
const STATUS_LABELS = {
  completo: 'Completo',
  parcial: 'Parcial',
  ccpp: 'Cubierto CCPP',
  sin_cupos: 'Sin cupos',
  sin_tope: 'Sin tope',
  bloq_op: 'Bloq. tipo op.',
  bloq_cls: 'Bloq. clase',
  bloq_cuit: 'Bloq. tope CUIT'
};

let S = {
  contracts: [],
  ccppByCuitMat: {},
  ccppByMat: {},
  allMats: [],
  stats: emptyStats(),
  limits: {},
  dayLimits: {},
  dayCount: 2,
  dayDates: [],
  multiDay: false,
  spreadDays: true,
  opPct: defaultOpPct(),
  clsPct: defaultClsPct(),
  useOp: false,
  useCls: false,
  dateFilter: defaultDateFilter(),
  prices: {},
  pricesEnabled: false,
  sustainFirst: false,
  excludeFason: false,
  excludeAgCompra: false,
  cupoKg: 30000,
  cuitMaxPct: 0.30,
  config: null,
  importMeta: {},
  lastResult: null,
  lastRendered: null
};

document.addEventListener('DOMContentLoaded', async function () {
  const todayInput = document.getElementById('today-date');
  if (todayInput) {
    todayInput.value = toInputDate(new Date());
  }

  bindUpload();
  updateActionButtons();

  $(document).on('input', '#pct-op input.pct-inp, #pct-cls input.pct-inp', onPctInputChange);
  $(document).on('click', '#res-tabs .tab', function () {
    switchTab($(this).data('target'), this);
  });

  try {
    await loadConfig();
  } catch (error) {
    showError(error.message || 'No se pudo cargar la configuración.');
  }
});

function defaultOpPct() {
  return { con_corredor: 0, acopiador: 0, productor: 0, otros: 0 };
}

function defaultClsPct() {
  return {
    'MP-Fason': 0,
    'MP-Prest/Devolución': 0,
    Fijo: 0,
    'Contrato Hijo': 0,
    'A Fijar': 0
  };
}

function defaultDateFilter() {
  return { enabled: false, fdMin: null, fdMax: null, fhMin: null, fhMax: null };
}

function emptyStats() {
  const op = {};
  const cls = {};
  OP_TYPES.forEach(function (item) { op[item.id] = { count: 0, kg: 0 }; });
  CLASS_ORDER.forEach(function (item) { cls[item] = { count: 0, kg: 0 }; });
  return {
    op: op,
    cls: cls,
    sust: { count: 0, kg: 0 },
    dates: { fdMin: null, fdMax: null, fhMin: null, fhMax: null }
  };
}

function bindUpload() {
  const uz = document.getElementById('uz');
  const fi = document.getElementById('fi');
  if (!uz || !fi) {
    return;
  }

  uz.addEventListener('click', function () { fi.click(); });
  uz.addEventListener('dragover', function (event) {
    event.preventDefault();
    uz.classList.add('dragover');
  });
  uz.addEventListener('dragleave', function () { uz.classList.remove('dragover'); });
  uz.addEventListener('drop', function (event) {
    event.preventDefault();
    uz.classList.remove('dragover');
    if (event.dataTransfer.files[0]) {
      handleFile(event.dataTransfer.files[0]);
    }
  });
  fi.addEventListener('change', function (event) {
    if (event.target.files[0]) {
      handleFile(event.target.files[0]);
    }
  });
}

function isPlainObject(value) {
  return Object.prototype.toString.call(value) === '[object Object]';
}

// Diccionarios cuyas claves son datos (nombre de material, "CUIT|Material") y no deben
// camelizarse recursivamente, ya que eso corrompería el casing original de esos valores.
const RAW_DICTIONARY_KEYS = ['ccppbycuitmat', 'ccppbymat'];

function camelizeKeys(value) {
  if (Array.isArray(value)) {
    return value.map(function (item) { return camelizeKeys(item); });
  }
  if (!isPlainObject(value)) {
    return value;
  }
  const result = {};
  Object.keys(value).forEach(function (key) {
    const camel = key.length ? key.charAt(0).toLowerCase() + key.slice(1) : key;
    if (RAW_DICTIONARY_KEYS.indexOf(camel.toLowerCase()) >= 0) {
      result[camel] = value[key];
    } else {
      result[camel] = camelizeKeys(value[key]);
    }
  });
  return result;
}

function pick(obj) {
  if (!obj) {
    return undefined;
  }
  for (let i = 1; i < arguments.length; i += 1) {
    const key = arguments[i];
    // Se usa el operador "in" (en vez de hasOwnProperty) porque debe funcionar tanto con
    // objetos planos (JSON camelizado) como con elementos del DOM, cuyas propiedades
    // (checked, value, etc.) est\u00E1n definidas en el prototype y no como propiedad own.
    if (key in obj && obj[key] !== undefined && obj[key] !== null) {
      return obj[key];
    }
  }
  return undefined;
}

function toNumber(value) {
  if (value === null || value === undefined || value === '') {
    return 0;
  }
  if (typeof value === 'number') {
    return Number.isFinite(value) ? value : 0;
  }
  const normalized = String(value).trim().replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
  const parsed = parseFloat(normalized);
  return Number.isFinite(parsed) ? parsed : 0;
}

function toBoolean(value) {
  return value === true || value === 'true' || value === 1 || value === '1';
}

function esc(value) {
  return String(value || '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');
}

function parseFlexibleDate(value) {
  if (!value) {
    return null;
  }
  if (value instanceof Date) {
    return new Date(value.getTime());
  }
  const stringValue = String(value).trim();
  if (!stringValue) {
    return null;
  }
  if (/^\d{4}-\d{2}-\d{2}$/.test(stringValue)) {
    const parts = stringValue.split('-').map(Number);
    return new Date(parts[0], parts[1] - 1, parts[2]);
  }
  if (/^\d{2}\/\d{2}\/\d{4}$/.test(stringValue)) {
    const parts = stringValue.split('/').map(Number);
    return new Date(parts[2], parts[1] - 1, parts[0]);
  }
  if (/^\d{2}\.\d{2}\.\d{4}$/.test(stringValue)) {
    const parts = stringValue.split('.').map(Number);
    return new Date(parts[2], parts[1] - 1, parts[0]);
  }
  const parsed = new Date(stringValue);
  return Number.isNaN(parsed.getTime()) ? null : parsed;
}

function fmt(date, separator) {
  if (!date) {
    return '—';
  }
  const sep = separator || '/';
  return [
    String(date.getDate()).padStart(2, '0'),
    String(date.getMonth() + 1).padStart(2, '0'),
    date.getFullYear()
  ].join(sep);
}

function toInputDate(date) {
  if (!date) {
    return '';
  }
  return [
    date.getFullYear(),
    String(date.getMonth() + 1).padStart(2, '0'),
    String(date.getDate()).padStart(2, '0')
  ].join('-');
}

function cloneDate(date) {
  return date ? new Date(date.getTime()) : null;
}

function addDays(date, days) {
  const copy = cloneDate(date);
  if (!copy) {
    return null;
  }
  copy.setDate(copy.getDate() + days);
  return copy;
}

function safeId(value) {
  return String(value || '').replace(/[^A-Za-z0-9_-]/g, '_');
}

function toLocaleNumber(value) {
  const numeric = Number(value || 0);
  return Number.isFinite(numeric) ? numeric.toLocaleString('es-AR') : '0';
}

function matCol(material) {
  const label = String(material || '').toLowerCase();
  if (label.indexOf('soja') >= 0) {
    return { cls: 'col-soja' };
  }
  if (label.indexOf('maiz') >= 0 || label.indexOf('maíz') >= 0) {
    return { cls: 'col-maiz' };
  }
  if (label.indexOf('trigo') >= 0) {
    return { cls: 'col-trigo' };
  }
  return { cls: 'col-otro' };
}

function classMeta(clase) {
  return CLASS_META[clase] || { label: clase || 'Sin clase', cls: 'p4' };
}

function opMeta(opType) {
  for (let i = 0; i < OP_TYPES.length; i += 1) {
    if (OP_TYPES[i].id === opType) {
      return OP_TYPES[i];
    }
  }
  return OP_TYPES[OP_TYPES.length - 1];
}

function pctWidthClass(value) {
  const width = Math.max(0, Math.min(100, Math.round(value || 0)));
  return 'pctw-' + width;
}

function dayColsClass(count) {
  const safeCount = Math.max(2, Math.min(7, count || 2));
  return 'lim-row-daycols-' + safeCount;
}

function getToday() {
  return parseFlexibleDate(pick(document.getElementById('today-date') || {}, 'value')) || new Date();
}

function toggleHidden(id, hidden) {
  const element = document.getElementById(id);
  if (element) {
    element.classList.toggle('hidden', hidden);
  }
}

function readJson(response) {
  return response.text().then(function (text) {
    if (!text) {
      return {};
    }
    try {
      return camelizeKeys(JSON.parse(text));
    } catch (error) {
      return { error: text };
    }
  });
}

function apiMessage(data, fallback) {
  return pick(data, 'error', 'mensaje', 'message') || fallback;
}

function showError(message) {
  alert(message || 'Ocurrió un error inesperado.');
}

function showSuccess(message) {
  alert(message || 'Operación realizada correctamente.');
}

function setCalcLoading(isLoading) {
  const button = document.getElementById('btn-calcular');
  document.body.classList.toggle('is-loading', isLoading);
  if (!button) {
    return;
  }
  button.disabled = isLoading;
  button.textContent = isLoading ? '⏳ Calculando distribución...' : '🔄 Calcular distribución';
}

function setBtnProcesar(state) {
  const button = document.getElementById('btn-procesar-dataagro');
  if (!button) {
    return;
  }
  button.dataset.state = state;
  if (state === 'loading') {
    button.disabled = true;
    button.textContent = '⏳ Procesando en DataAgro...';
  } else {
    button.textContent = '⚡ Procesar en DataAgro';
    updateActionButtons();
  }
}

function extractSapData() {
  const rows = pick(S.lastResult, 'sap', 'sapConsolidado') || [];
  if (!Array.isArray(rows)) {
    return [];
  }
  return rows.map(function (row) {
    const item = camelizeKeys(row || {});
    return {
      fechaSugerida: pick(item, 'fechaSugerida') || '',
      cantidadDeCupos: toNumber(pick(item, 'cantidadDeCupos', 'cuposAsignados')),
      contratoSAP: String(pick(item, 'contratoSAP', 'numeroSap', 'numeroSAP') || '')
    };
  }).filter(function (row) {
    return row.contratoSAP && row.cantidadDeCupos > 0;
  });
}

function updateActionButtons() {
  const hasSap = extractSapData().length > 0;
  const exportButton = document.getElementById('btn-exportar');
  const processButton = document.getElementById('btn-procesar-dataagro');
  const copyButton = document.getElementById('btn-copiar');

  if (exportButton) {
    exportButton.disabled = !hasSap;
  }
  if (copyButton) {
    copyButton.disabled = !hasSap;
  }
  if (processButton) {
    processButton.disabled = !hasSap || processButton.dataset.state === 'loading';
  }
}

async function handleFile(file) {
  document.getElementById('fname').textContent = file.name;
  document.getElementById('fmeta').textContent = 'Importando...';
  toggleHidden('uz', true);
  toggleHidden('file-info', false);
  toggleHidden('card-results', true);
  toggleHidden('card-export', true);
  S.lastResult = null;
  S.lastRendered = null;
  updateActionButtons();

  const formData = new FormData();
  formData.append('file', file);
  formData.append('fecha', toInputDate(getToday()));

  try {
    const response = await fetch(API_IMPORTAR, { method: 'POST', body: formData });
    const data = await readJson(response);
    console.log('Importar response:', { ok: response.ok, data: data });
    if (!response.ok) {
      throw new Error(apiMessage(data, 'No se pudo importar el archivo.'));
    }
    S.importMeta.fileName = file.name;
    applyParsed(data);
    buildAll();
    toggleHidden('card-limits', false);
    toggleHidden('multiday-section', false);
  } catch (error) {
    console.error('Error en handleFile:', error);
    showError(error.message || 'No se pudo leer el archivo.');
    removeFile();
  }
}

function normalizeStats(stats, contracts) {
  const normalized = camelizeKeys(stats || {});
  const base = emptyStats();

  const porTipoOperador = Array.isArray(normalized.porTipoOperador) ? normalized.porTipoOperador : [];
  porTipoOperador.forEach(function (item) {
    const id = pick(item, 'tipoId');
    if (id && Object.prototype.hasOwnProperty.call(base.op, id)) {
      base.op[id] = { count: toNumber(pick(item, 'contratos')), kg: toNumber(pick(item, 'kgTotal')) };
    }
  });

  const porClase = Array.isArray(normalized.porClase) ? normalized.porClase : [];
  porClase.forEach(function (item) {
    const clase = pick(item, 'clase');
    if (clase && Object.prototype.hasOwnProperty.call(base.cls, clase)) {
      base.cls[clase] = { count: toNumber(pick(item, 'contratos')), kg: toNumber(pick(item, 'kgTotal')) };
    }
  });

  const sustContracts = (contracts || []).filter(function (contract) { return toBoolean(contract.isSust); });
  base.sust = {
    count: toNumber(pick(normalized, 'sustentables')) || sustContracts.length,
    kg: sustContracts.reduce(function (sum, contract) { return sum + toNumber(contract.kg); }, 0)
  };
  base.dates = {
    fdMin: parseFlexibleDate(pick(normalized, 'fechaDesdeMin')),
    fdMax: parseFlexibleDate(pick(normalized, 'fechaDesdeMax')),
    fhMin: parseFlexibleDate(pick(normalized, 'fechaHastaMin')),
    fhMax: parseFlexibleDate(pick(normalized, 'fechaHastaMax'))
  };
  return base;
}

// Normaliza la respuesta importada y deja el estado listo para la UI.
function applyParsed(data) {
  const parsed = camelizeKeys(data || {});
  S.contracts = Array.isArray(parsed.contratos) ? parsed.contratos : [];
  S.ccppByCuitMat = parsed.ccppByCuitMat || {};
  S.ccppByMat = parsed.ccppByMat || {};
  S.allMats = (Array.isArray(parsed.materiales) && parsed.materiales.length ? parsed.materiales : Object.keys(S.ccppByMat))
    .concat(S.contracts.map(function (contract) { return contract.material; }))
    .filter(function (value, index, array) { return value && array.indexOf(value) === index; })
    .sort(function (a, b) { return String(a).localeCompare(String(b), 'es'); });
  S.stats = normalizeStats(parsed.estadisticas || parsed.stats || {}, S.contracts);
  S.importMeta.totalFilas = toNumber(pick(parsed.estadisticas || {}, 'totalFilas', 'cantidadFilas')) || toNumber(parsed.totalFilas) || S.contracts.length;

  const defaultLimits = pick(S.config || {}, 'limitesPredeterminadosPorMaterial') || {};
  const defaultPrices = pick(S.config || {}, 'preciosReferencia') || {};
  S.allMats.forEach(function (mat) {
    if (S.limits[mat] === undefined) {
      S.limits[mat] = toNumber(defaultLimits[mat]);
    }
    if (!Object.prototype.hasOwnProperty.call(S.prices, mat)) {
      S.prices[mat] = toNumber(defaultPrices[mat]);
    }
  });

  document.getElementById('fmeta').textContent =
    S.importMeta.totalFilas.toLocaleString('es-AR') + ' filas · ' +
    S.contracts.length + ' contratos válidos · ' + S.allMats.length + ' materiales';
}

function removeFile() {
  toggleHidden('uz', false);
  toggleHidden('file-info', true);
  toggleHidden('card-limits', true);
  toggleHidden('card-results', true);
  toggleHidden('card-export', true);
  document.getElementById('fi').value = '';
  S.contracts = [];
  S.ccppByCuitMat = {};
  S.ccppByMat = {};
  S.allMats = [];
  S.stats = emptyStats();
  S.dayLimits = {};
  S.lastResult = null;
  S.lastRendered = null;
  updateActionButtons();
}

function buildAll() {
  syncStaticUi();
  updateDayDates();
  buildLimGrid();
  buildStatsUI();
  buildPctInputs();
  buildDateFilterUI();
  buildPricesUI();
  onToggleOp();
  onToggleCls();
}

function syncStaticUi() {
  const chkOp = document.getElementById('chk-op');
  const chkCls = document.getElementById('chk-cls');
  const chkMulti = document.getElementById('chk-multiday');
  const chkSpread = document.getElementById('chk-spread');
  const chkFason = document.getElementById('chk-excl-fason');
  const chkAgCompra = document.getElementById('chk-excl-agcompra');
  const dayCount = document.getElementById('day-count');
  if (chkOp) { chkOp.checked = S.useOp; }
  if (chkCls) { chkCls.checked = S.useCls; }
  if (chkMulti) { chkMulti.checked = S.multiDay; }
  if (chkSpread) { chkSpread.checked = S.spreadDays; }
  if (chkFason) { chkFason.checked = S.excludeFason; }
  if (chkAgCompra) { chkAgCompra.checked = S.excludeAgCompra; }
  if (dayCount) { dayCount.value = String(S.dayCount || 2); }
  updateExclBadge('fason');
  updateExclBadge('agcompra');
  onToggleMultiDay();
}

// Crea la grilla de topes diarios y por material para la configuración.
function buildLimGrid() {
  const container = $('#grid-limites');
  if (!container.length) {
    return;
  }
  const cupoKg = S.cupoKg || 30000;

  if (!S.multiDay || S.dayDates.length < 2) {
    const rows = S.allMats.map(function (mat) {
      const ccppKg = toNumber(S.ccppByMat[mat]);
      return {
        material: mat,
        colClass: matCol(mat).cls,
        limit: S.limits[mat] !== undefined ? S.limits[mat] : 0,
        ccppCupos: Math.ceil(ccppKg / cupoKg),
        ccppTon: ccppKg / 1000
      };
    });
    renderDataGrid('grid-limites', rows, [
      { field: 'material', title: 'Material', template: function (row) { return '<span class="dot ' + row.colClass + '"></span>' + esc(row.material); } },
      {
        field: 'limit',
        title: 'Tope diario (cupos totales)',
        template: function (row) {
          return '<input type="number" min="0" step="1" class="lim-input" data-material="' + esc(row.material) + '" value="' + row.limit + '">';
        }
      },
      {
        field: 'ccppCupos',
        title: 'CCPP pendiente',
        template: function (row) {
          return 'CCPP pendiente: <strong class="txt-org">' + row.ccppCupos.toLocaleString('es-AR') + ' cupos</strong>' +
            '<span class="d-block fs-11">' + row.ccppTon.toLocaleString('es-AR', { maximumFractionDigits: 0 }) + ' t — descontado por CUIT</span>';
        }
      }
    ], {
      sortable: false,
      dataBound: function () {
        this.element.find('input.lim-input').on('input', function () {
          const material = $(this).data('material');
          S.limits[material] = parseInt(this.value, 10) || 0;
        });
      }
    });
    return;
  }

  const rows = S.allMats.map(function (mat) {
    const ccppKg = toNumber(S.ccppByMat[mat]);
    const defaultDayLimit = toNumber(S.limits[mat]);
    if (!Array.isArray(S.dayLimits[mat])) {
      S.dayLimits[mat] = Array(S.dayDates.length).fill(defaultDayLimit);
    } else {
      while (S.dayLimits[mat].length < S.dayDates.length) {
        S.dayLimits[mat].push(defaultDayLimit);
      }
      S.dayLimits[mat] = S.dayLimits[mat].slice(0, S.dayDates.length).map(function (value, index) {
        return toNumber(value) || (index === 0 ? defaultDayLimit : 0);
      });
    }
    const row = {
      material: mat,
      colClass: matCol(mat).cls,
      ccppCupos: Math.ceil(ccppKg / cupoKg),
      ccppTon: ccppKg / 1000
    };
    S.dayDates.forEach(function (_, index) {
      row['day' + index] = toNumber(S.dayLimits[mat][index]);
    });
    return row;
  });

  const dayColumns = S.dayDates.map(function (day, index) {
    return {
      field: 'day' + index,
      title: 'Día ' + (index + 1),
      headerTemplate: '<div class="lim-day-header"><div>Día ' + (index + 1) + '</div><div class="fw-400">' + fmt(day, '/') + '</div></div>',
      encoded: false,
      attributes: { class: 'text-center' },
      template: function (row) {
        return '<input class="w-100 input-center lim-day-input" type="number" min="0" step="1" data-material="' +
          esc(row.material) + '" data-day="' + index + '" value="' + row['day' + index] + '">';
      }
    };
  });

  renderDataGrid('grid-limites', rows, [
    { field: 'material', title: 'Material', template: function (row) { return '<span class="dot ' + row.colClass + '"></span>' + esc(row.material); } }
  ].concat(dayColumns).concat([
    {
      field: 'ccppCupos',
      title: 'CCPP pendiente',
      template: function (row) {
        return '<strong class="txt-org">' + row.ccppCupos.toLocaleString('es-AR') + ' cupos</strong>' +
          '<span class="d-block fs-11">' + row.ccppTon.toLocaleString('es-AR', { maximumFractionDigits: 0 }) + ' t</span>';
      }
    }
  ]), {
    sortable: false,
    dataBound: function () {
      this.element.find('input.lim-day-input').on('input', function () {
        const material = $(this).data('material');
        const dayIndex = $(this).data('day');
        if (!Array.isArray(S.dayLimits[material])) {
          S.dayLimits[material] = [];
        }
        S.dayLimits[material][dayIndex] = parseInt(this.value, 10) || 0;
      });
    }
  });
}

function buildStatsUI() {
  const totalKg = S.contracts.reduce(function (sum, contract) { return sum + toNumber(contract.kg); }, 0) || 1;

  const opRows = OP_TYPES.map(function (item) {
    const stat = S.stats.op[item.id] || { count: 0, kg: 0 };
    const pct = stat.kg / totalKg * 100;
    return { label: item.label, colorClass: item.colorClass, count: stat.count, kgM: stat.kg / 1e6, pct: pct };
  });
  renderStatsGrid('grid-stats-op', opRows);

  const clsRows = CLASS_ORDER.map(function (clase) {
    const stat = S.stats.cls[clase] || { count: 0, kg: 0 };
    const meta = classMeta(clase);
    const pct = stat.kg / totalKg * 100;
    return { label: meta.label, badgeClass: meta.cls, count: stat.count, kgM: stat.kg / 1e6, pct: pct };
  });
  renderStatsGrid('grid-stats-cls', clsRows);
}

function renderDataGrid(elementId, rows, columns, options) {
  const element = $('#' + elementId);
  if (!element.length) {
    return null;
  }
  const existing = element.data('kendoGrid');
  if (existing) {
    existing.setOptions({ columns: columns });
    existing.dataSource.data(rows);
    return existing;
  }
  const config = Object.assign({
    dataSource: { data: rows },
    scrollable: false,
    sortable: true,
    columns: columns
  }, options || {});
  return element.kendoGrid(config).data('kendoGrid');
}

function renderStatsGrid(elementId, rows) {
  renderDataGrid(elementId, rows, [
    {
      field: 'label',
      title: rows[0] && rows[0].badgeClass !== undefined ? 'Clase' : 'Tipo',
      template: function (row) {
        return row.badgeClass
          ? '<span class="bp ' + row.badgeClass + '">' + esc(row.label) + '</span>'
          : '<span class="dot ' + row.colorClass + '"></span> ' + esc(row.label);
      }
    },
    { field: 'count', title: 'Contratos', attributes: { class: 'r' } },
    { field: 'kgM', title: 'Kg (M)', attributes: { class: 'r' }, template: function (row) { return row.kgM.toFixed(1); } },
    {
      field: 'pct',
      title: '%',
      attributes: { class: 'r' },
      template: function (row) {
        const barClass = row.badgeClass ? 'bar-g' : row.colorClass;
        return row.pct.toFixed(1) + '%<span class="bar-bg"><span class="bar-fg ' + barClass + ' ' + pctWidthClass(row.pct) + '"></span></span>';
      }
    }
  ], { sortable: false, scrollable: false });
}

function filterGrid(elementId, query) {
  const grid = $('#' + elementId).data('kendoGrid');
  if (!grid) {
    return;
  }
  const value = String(query || '').trim();
  if (!value) {
    grid.dataSource.filter({});
    return;
  }
  const filters = grid.columns
    .filter(function (column) { return column.field; })
    .map(function (column) { return { field: column.field, operator: 'contains', value: value }; });
  grid.dataSource.filter({ logic: 'or', filters: filters });
}

function getPctRowTemplate() {
  if (!S._pctRowTemplate) {
    S._pctRowTemplate = kendo.template($('#tmpl-pct-row').html());
  }
  return S._pctRowTemplate;
}

function buildPctInputs() {
  const pctOp = $('#pct-op');
  const pctCls = $('#pct-cls');
  const template = getPctRowTemplate();

  if (pctOp.length) {
    const opData = OP_TYPES.map(function (item) {
      return {
        kind: 'op',
        key: item.id,
        value: toNumber(S.opPct[item.id]),
        iconHtml: '<span class="dot ' + item.colorClass + '"></span>' + esc(item.label)
      };
    });
    pctOp.html(kendo.render(template, opData));
  }

  if (pctCls.length) {
    const clsData = CLASS_ORDER.map(function (clase) {
      const meta = classMeta(clase);
      return {
        kind: 'cls',
        key: clase,
        value: toNumber(S.clsPct[clase]),
        iconHtml: '<span class="bp ' + meta.cls + '">' + esc(meta.label) + '</span>'
      };
    });
    pctCls.html(kendo.render(template, clsData));
  }

  updatePctTotal('op');
  updatePctTotal('cls');
}

function onPctInputChange(event) {
  const input = $(event.target);
  const kind = input.data('kind');
  const key = String(input.data('key'));
  const value = parseInt(input.val(), 10) || 0;
  if (kind === 'op') {
    S.opPct[key] = value;
  } else {
    S.clsPct[key] = value;
  }
  updatePctTotal(kind);
}

function updatePctTotal(which) {
  if (which === 'op') {
    const total = Object.keys(S.opPct).reduce(function (sum, key) { return sum + toNumber(S.opPct[key]); }, 0);
    const element = document.getElementById('pct-op-total');
    if (element) {
      element.textContent = 'Total: ' + total + '% ' + (total <= 100 ? '(' + (100 - total) + '% libre)' : '⚠️ excede 100%');
      element.className = 'pct-total' + (total > 100 ? ' warn' : '');
    }
    return;
  }
  const total = Object.keys(S.clsPct).reduce(function (sum, key) { return sum + toNumber(S.clsPct[key]); }, 0);
  const element = document.getElementById('pct-cls-total');
  if (element) {
    element.textContent = 'Total: ' + total + '% ' + (total <= 100 ? '(' + (100 - total) + '% libre)' : '⚠️ excede 100%');
    element.className = 'pct-total' + (total > 100 ? ' warn' : '');
  }
}

function onToggleOp() {
  S.useOp = $('#chk-op').is(':checked');
  $('#pct-op-wrap').toggleClass('hidden', !S.useOp);
  $('#pct-op-off').toggleClass('hidden', S.useOp);
}

function onToggleCls() {
  S.useCls = $('#chk-cls').is(':checked');
  $('#pct-cls-wrap').toggleClass('hidden', !S.useCls);
  $('#pct-cls-off').toggleClass('hidden', S.useCls);
}

function toggleSection(sectionId, dividerId) {
  const divider = $('#' + dividerId);
  if (!divider.length) {
    return;
  }
  const isOpen = divider.hasClass('open');
  divider.toggleClass('open', !isOpen);
  $('#' + sectionId).toggleClass('hidden', isOpen);
}

// Muestra los rangos reales del archivo y activa el filtro de fechas opcional.
function buildDateFilterUI() {
  const container = document.getElementById('date-filter-ui');
  if (!container) {
    return;
  }
  const dates = S.stats.dates || {};
  container.innerHTML = renderTemplate('tmpl-date-filter-ui', {
    enabled: !!S.dateFilter.enabled,
    dateFilter: {
      fdMin: toInputDate(S.dateFilter.fdMin),
      fdMax: toInputDate(S.dateFilter.fdMax),
      fhMin: toInputDate(S.dateFilter.fhMin),
      fhMax: toInputDate(S.dateFilter.fhMax)
    }
  });
  const checkbox = document.getElementById('chk-date');
  if (checkbox) {
    checkbox.checked = S.dateFilter.enabled;
  }
  onToggleDate();
  renderDataGrid('grid-dates', [
    { campo: 'Fecha Desde', min: fmt(dates.fdMin, '.'), max: fmt(dates.fdMax, '.') },
    { campo: 'Fecha Hasta', min: fmt(dates.fhMin, '.'), max: fmt(dates.fhMax, '.') }
  ], [
    { field: 'campo', title: 'Campo' },
    { field: 'min', title: 'Mínimo', attributes: { class: 'r' } },
    { field: 'max', title: 'Máximo', attributes: { class: 'r' } }
  ], { sortable: false, pageable: false });
}

function onToggleDate() {
  S.dateFilter.enabled = $('#chk-date').is(':checked');
  $('#date-filter-wrap').toggleClass('hidden', !S.dateFilter.enabled);
  $('#date-filter-off').toggleClass('hidden', S.dateFilter.enabled);
}

function onDateFilterChange() {
  S.dateFilter.fdMin = parseFlexibleDate(pick(document.getElementById('df-fdMin') || {}, 'value'));
  S.dateFilter.fdMax = parseFlexibleDate(pick(document.getElementById('df-fdMax') || {}, 'value'));
  S.dateFilter.fhMin = parseFlexibleDate(pick(document.getElementById('df-fhMin') || {}, 'value'));
  S.dateFilter.fhMax = parseFlexibleDate(pick(document.getElementById('df-fhMax') || {}, 'value'));
}

// Configura la prioridad por sustentabilidad y precio de referencia.
function buildPricesUI() {
  const container = document.getElementById('prices-sust-ui');
  if (!container) {
    return;
  }
  const sust = S.stats.sust || { count: 0, kg: 0 };
  const totalKg = S.contracts.reduce(function (sum, contract) { return sum + toNumber(contract.kg); }, 0) || 1;
  const sustPct = ((toNumber(sust.kg) / totalKg) * 100).toFixed(1);
  container.innerHTML = renderTemplate('tmpl-prices-sust-ui', {
    sustainFirst: !!S.sustainFirst,
    pricesEnabled: !!S.pricesEnabled
  });

  const sustain = document.getElementById('chk-sust');
  const prices = document.getElementById('chk-prices');
  if (sustain) { sustain.checked = S.sustainFirst; }
  if (prices) { prices.checked = S.pricesEnabled; }
  onTogglePrices();

  renderDataGrid('grid-sust', [
    { indicador: 'Contratos con Sust.=X o EPA=X', valor: toNumber(sust.count), cls: 'txt-green' },
    { indicador: 'Kg sustentables (M)', valor: (toNumber(sust.kg) / 1e6).toFixed(2) + ' M kg' },
    { indicador: '% del total kg', valor: sustPct + '%' }
  ], [
    { field: 'indicador', title: 'Indicador' },
    { field: 'valor', title: 'Valor', attributes: { class: 'r' }, template: function (row) { return row.cls ? '<strong class="' + row.cls + '">' + row.valor + '</strong>' : row.valor; } }
  ], { sortable: false, pageable: false });

  const priceRows = S.allMats.map(function (mat) {
    return { material: mat, colClass: matCol(mat).cls, price: S.prices[mat] || '' };
  });
  renderDataGrid('grid-prices', priceRows, [
    { field: 'material', title: 'Material', template: function (row) { return '<span class="dot ' + row.colClass + '"></span>' + esc(row.material); } },
    {
      field: 'price',
      title: 'Precio ref. (USD/tn)',
      template: function (row) {
        return '<input type="number" min="0" step="0.01" class="price-input" data-material="' + esc(row.material) + '" value="' + row.price + '" placeholder="ej: 280">';
      }
    },
    { field: 'material', title: '', template: function () { return 'Contratos Fijo/Hijo con precio ≥ ref.<br><span class="fs-11">se priorizan dentro de su clase</span>'; } }
  ], {
    sortable: false,
    pageable: false,
    dataBound: function () {
      this.element.find('input.price-input').on('input', function () {
        const material = $(this).data('material');
        S.prices[material] = parseFloat(this.value) || 0;
      });
    }
  });
}

function onTogglePrices() {
  S.pricesEnabled = $('#chk-prices').is(':checked');
  $('#prices-wrap').toggleClass('hidden', !S.pricesEnabled);
  $('#prices-off').toggleClass('hidden', S.pricesEnabled);
}

function updateExclBadge(which) {
  if (which === 'fason') {
    S.excludeFason = $('#chk-excl-fason').is(':checked');
    $('#lbl-excl-fason').toggleClass('excl-on-red', S.excludeFason);
    return;
  }
  S.excludeAgCompra = $('#chk-excl-agcompra').is(':checked');
  $('#lbl-excl-agcompra').toggleClass('excl-on-purple', S.excludeAgCompra);
}

function onToggleMultiDay() {
  S.multiDay = $('#chk-multiday').is(':checked');
  $('#multiday-config').toggleClass('hidden', !S.multiDay);
  updateDayDates();
  buildLimGrid();
}

function onDayCountChange() {
  const value = parseInt($('#day-count').val(), 10) || 2;
  S.dayCount = Math.max(2, Math.min(7, value));
  updateDayDates();
  buildLimGrid();
}

function updateDayDates() {
  const count = parseInt(pick(document.getElementById('day-count') || {}, 'value'), 10) || S.dayCount || 2;
  S.dayCount = Math.max(2, Math.min(7, count));
  S.spreadDays = !!pick(document.getElementById('chk-spread') || {}, 'checked');
  S.dayDates = [];
  if (S.multiDay) {
    const baseDate = getToday();
    for (let i = 0; i < S.dayCount; i += 1) {
      S.dayDates.push(addDays(baseDate, i));
    }
  }
  const preview = document.getElementById('day-dates-preview');
  if (preview) {
    if (!S._dayBadgeTemplate) {
      S._dayBadgeTemplate = kendo.template($('#tmpl-day-badge').html());
    }
    const data = S.dayDates.map(function (day, index) {
      return { index: index, label: fmt(day, '/') };
    });
    preview.innerHTML = kendo.render(S._dayBadgeTemplate, data);
  }
}

function readSingleDayLimits() {
  const limits = {};
  S.allMats.forEach(function (mat) {
    limits[mat] = toNumber(S.limits[mat]);
  });
  S.limits = limits;
  return limits;
}

function readMultiDayLimits() {
  const limitsByDay = [];
  const totalByMaterial = {};
  S.allMats.forEach(function (mat) {
    totalByMaterial[mat] = 0;
    if (!Array.isArray(S.dayLimits[mat])) {
      S.dayLimits[mat] = Array(S.dayDates.length).fill(toNumber(S.limits[mat]));
    }
    while (S.dayLimits[mat].length < S.dayDates.length) {
      S.dayLimits[mat].push(toNumber(S.limits[mat]));
    }
    S.dayLimits[mat] = S.dayLimits[mat].slice(0, S.dayDates.length).map(function (value) {
      const numeric = toNumber(value);
      return numeric > 0 ? numeric : toNumber(S.limits[mat]);
    });
  });
  S.dayDates.forEach(function (day, dayIndex) {
    const dayLimits = {};
    S.allMats.forEach(function (mat) {
      const value = toNumber((S.dayLimits[mat] || [])[dayIndex]);
      dayLimits[mat] = value;
      totalByMaterial[mat] += value;
    });
    limitsByDay.push({ fecha: toInputDate(day), limites: dayLimits });
  });
  S.limits = totalByMaterial;
  return limitsByDay;
}

function buildExcludedClasses() {
  return S.excludeFason ? ['MP-Fason'] : [];
}

// Genera la config que se envía al backend para respetar reglas y filtros.
function buildDistributionConfig() {
  return {
    cupoKg: S.cupoKg,
    cuitMaxPct: S.cuitMaxPct,
    aplicarCuotaOperador: S.useOp,
    cuotasPorOperador: S.opPct,
    aplicarCuotaClase: S.useCls,
    cuotasPorClase: S.clsPct,
    excluirFason: S.excludeFason,
    excluirAgenteCompra: S.excludeAgCompra,
    priorizarSustentables: S.sustainFirst,
    preciosReferencia: S.pricesEnabled ? S.prices : {},
    habilitarFiltroFechas: S.dateFilter.enabled,
    fdMin: toInputDate(S.dateFilter.fdMin),
    fdMax: toInputDate(S.dateFilter.fdMax),
    fhMin: toInputDate(S.dateFilter.fhMin),
    fhMax: toInputDate(S.dateFilter.fhMax),
    fechaDesdeMin: toInputDate(S.dateFilter.fdMin),
    fechaDesdeMax: toInputDate(S.dateFilter.fdMax),
    fechaHastaMin: toInputDate(S.dateFilter.fhMin),
    fechaHastaMax: toInputDate(S.dateFilter.fhMax)
  };
}

// Arma el payload del cálculo para un solo día de distribución.
function buildSingleDayRequest() {
  return {
    fecha: toInputDate(getToday()),
    contratos: S.contracts,
    ccppByCuitMat: S.ccppByCuitMat,
    limitesPorMaterial: readSingleDayLimits(),
    configuracion: buildDistributionConfig()
  };
}

// Arma el payload con límites por día y reparto uniforme opcional.
function buildMultiDayRequest() {
  return {
    contratos: S.contracts,
    ccppByCuitMat: S.ccppByCuitMat,
    fechas: S.dayDates.map(toInputDate),
    limitesPorDia: readMultiDayLimits(),
    distribuirUniforme: S.spreadDays,
    configuracion: buildDistributionConfig()
  };
}

async function runDistribution() {
  if (!S.contracts.length) {
    showError('Primero importá un archivo SAP válido.');
    return;
  }

  onDateFilterChange();
  S.multiDay = !!pick(document.getElementById('chk-multiday') || {}, 'checked');
  S.spreadDays = !!pick(document.getElementById('chk-spread') || {}, 'checked');
  S.sustainFirst = !!pick(document.getElementById('chk-sust') || {}, 'checked');
  S.pricesEnabled = !!pick(document.getElementById('chk-prices') || {}, 'checked');
  updateExclBadge('fason');
  updateExclBadge('agcompra');
  updateDayDates();

  const isMulti = S.multiDay && S.dayDates.length > 1;
  const url = isMulti ? API_CALCULAR_MD : API_CALCULAR;
  const body = isMulti ? buildMultiDayRequest() : buildSingleDayRequest();

  setCalcLoading(true);
  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    });
    const data = await readJson(response);
    if (!response.ok) {
      throw new Error(apiMessage(data, 'No se pudo calcular la distribución.'));
    }
    S.lastResult = data;
    renderResults(normalizeDistributionResult(data));
    toggleHidden('card-results', false);
    toggleHidden('card-export', false);
    document.getElementById('card-results').scrollIntoView({ behavior: 'smooth' });
    saveConfig(false).catch(function () { return null; });
  } catch (error) {
    showError(error.message || 'Error al calcular la distribución.');
  } finally {
    setCalcLoading(false);
    updateActionButtons();
  }
}

function findSourceContract(row) {
  const numeroSap = String(pick(row, 'numeroSap', 'numeroSAP', 'contratoSAP') || '');
  for (let i = 0; i < S.contracts.length; i += 1) {
    const contract = S.contracts[i];
    if (String(contract.numeroSAP || contract.numeroSap || '') === numeroSap) {
      return contract;
    }
  }
  return null;
}

function decorateResultRow(row, assignedDate) {
  const source = findSourceContract(row) || {};
  const clase = pick(row, 'clase', 'descCl') || pick(source, 'descCl', 'clase') || '';
  const meta = classMeta(clase);
  const opType = pick(row, 'opType') || pick(source, 'opType') || 'otros';
  const operator = opMeta(opType);
  const kgContrato = toNumber(pick(row, 'kgContrato', 'kg', 'kgTotal')) || toNumber(pick(source, 'kg'));
  const ccppDescontado = toNumber(pick(row, 'ccppDescontado', 'ccppKgUsed'));
  const kgEfectivo = toNumber(pick(row, 'kgEfectivo', 'effKg')) || Math.max(0, kgContrato - ccppDescontado);
  const cuposAsignados = toNumber(pick(row, 'cuposAsignados', 'cuposAssigned'));
  const cuposNecesarios = toNumber(pick(row, 'cuposNecesarios', 'cuposNeeded'));
  return {
    numeroSAP: String(pick(row, 'contratoSAP', 'numeroSap', 'numeroSAP') || pick(source, 'numeroSAP', 'numeroSap') || ''),
    prov: pick(row, 'proveedor', 'prov') || pick(source, 'prov', 'proveedor') || '',
    cuit: pick(row, 'cuit') || pick(source, 'cuit') || '',
    material: pick(row, 'material') || pick(source, 'material') || '',
    opType: opType,
    opLabel: pick(row, 'opLabel') || operator.label,
    descCl: clase,
    prioLabel: pick(row, 'prioLabel') || meta.label,
    prioCls: pick(row, 'prioCls') || meta.cls,
    kg: kgContrato,
    ccppKgUsed: ccppDescontado,
    effKg: kgEfectivo,
    cuposNeeded: cuposNecesarios,
    cuposAssigned: cuposAsignados,
    status: pick(row, 'estado', 'status') || (cuposAsignados > 0 ? 'completo' : 'sin_cupos'),
    isSust: toBoolean(pick(row, 'isSust')) || toBoolean(pick(source, 'isSust')),
    pricePt: toNumber(pick(row, 'pricePt')) || toNumber(pick(source, 'pricePt')),
    cosecha: pick(row, 'cosecha') || pick(source, 'cosecha') || '',
    fechaHasta: parseFlexibleDate(pick(row, 'fechaHasta')) || parseFlexibleDate(pick(source, 'fechaHasta')),
    fechaAsignada: parseFlexibleDate(pick(row, 'fechaSugerida', 'fechaAsignada')) || assignedDate || getToday()
  };
}

function mergeRows(rows) {
  const map = {};
  rows.forEach(function (row) {
    const key = [row.numeroSAP, row.material, row.cuit, row.descCl, row.opType].join('|');
    if (!map[key]) {
      map[key] = Object.assign({}, row);
      return;
    }
    map[key].cuposAssigned += row.cuposAssigned;
    map[key].status = map[key].cuposNeeded > 0 && map[key].cuposAssigned >= map[key].cuposNeeded ? 'completo' : map[key].status;
  });
  return Object.keys(map).map(function (key) { return map[key]; });
}

function calcRemaining(limits, rows) {
  const assigned = {};
  const remaining = {};
  rows.forEach(function (row) {
    assigned[row.material] = (assigned[row.material] || 0) + row.cuposAssigned;
  });
  Object.keys(limits || {}).forEach(function (mat) {
    remaining[mat] = Math.max(0, toNumber(limits[mat]) - toNumber(assigned[mat]));
  });
  return remaining;
}

function remainingFromSummary(summary, fallbackRows) {
  const remaining = {};
  let hasSummary = false;
  (summary || []).forEach(function (item) {
    const row = camelizeKeys(item || {});
    const material = pick(row, 'material');
    if (!material) {
      return;
    }
    hasSummary = true;
    S.limits[material] = toNumber(pick(row, 'limite')) || toNumber(S.limits[material]);
    remaining[material] = toNumber(pick(row, 'cuposRestantes'));
  });
  return hasSummary ? remaining : calcRemaining(S.limits, fallbackRows || []);
}

function normalizeDistributionResult(data) {
  const normalized = camelizeKeys(data || {});
  if (Array.isArray(normalized.resultadosPorDia) && normalized.resultadosPorDia.length) {
    const lines = [];
    normalized.resultadosPorDia.forEach(function (dayResult) {
      const assignedDate = parseFlexibleDate(pick(dayResult, 'fecha'));
      const dayRows = Array.isArray(dayResult.resultados) ? dayResult.resultados : [];
      dayRows.forEach(function (row) {
        const decorated = decorateResultRow(row, assignedDate);
        if (decorated.cuposAssigned > 0) {
          lines.push(decorated);
        }
      });
    });
    const summaryRows = mergeRows(lines);
    return {
      outputRows: lines,
      summaryRows: summaryRows,
      remaining: calcRemaining(S.limits, summaryRows)
    };
  }
  const summaryRows = (Array.isArray(normalized.resultados) ? normalized.resultados : []).map(function (row) {
    return decorateResultRow(row, parseFlexibleDate(pick(normalized, 'fecha')) || getToday());
  });
  return {
    outputRows: summaryRows.filter(function (row) { return row.cuposAssigned > 0; }),
    summaryRows: summaryRows,
    remaining: remainingFromSummary(normalized.resumenPorMaterial, summaryRows)
  };
}

function getTemplateById(id) {
  if (!S._tplCache) {
    S._tplCache = {};
  }
  if (!S._tplCache[id]) {
    const node = document.getElementById(id);
    if (!node) {
      return null;
    }
    S._tplCache[id] = kendo.template(node.innerHTML);
  }
  return S._tplCache[id];
}

function renderTemplate(id, data) {
  const template = getTemplateById(id);
  if (!template) {
    return '';
  }
  return template(data);
}

function getResTabTemplate() {
  if (!S._resTabTemplate) {
    S._resTabTemplate = kendo.template($('#tmpl-res-tab').html());
  }
  return S._resTabTemplate;
}

function getResTabContentTemplate() {
  if (!S._resTabContentTemplate) {
    S._resTabContentTemplate = kendo.template($('#tmpl-res-tab-content').html());
  }
  return S._resTabContentTemplate;
}

// Re-renderiza la pantalla con tabs, resúmenes y detalle por material.
function renderResults(result) {
  S.lastRendered = result;
  const outputRows = result.outputRows || [];
  const summaryRows = result.summaryRows || [];
  const totalAssigned = outputRows.reduce(function (sum, row) { return sum + row.cuposAssigned; }, 0);
  document.getElementById('res-badge').textContent = outputRows.length + ' líneas · ' + totalAssigned.toLocaleString('es-AR') + ' cupos';

  const byMaterial = {};
  S.allMats.forEach(function (mat) { byMaterial[mat] = []; });
  summaryRows.forEach(function (row) {
    if (!byMaterial[row.material]) {
      byMaterial[row.material] = [];
    }
    byMaterial[row.material].push(row);
  });

  const tabDefs = [{ id: 'output', label: '📋 Cupos a asignar', iconHtml: '', active: true }];
  tabDefs.push({ id: 'resumen', label: '📊 Resumen', iconHtml: '', active: false });

  if (S.useOp) {
    tabDefs.push({ id: 'op_tab', label: '🏢 Por tipo operador', iconHtml: '', active: false });
  }
  if (S.useCls) {
    tabDefs.push({ id: 'cls_tab', label: '📋 Por clase contrato', iconHtml: '', active: false });
  }
  S.allMats.forEach(function (mat) {
    const id = 'mat_' + safeId(mat);
    const label = mat.length > 20 ? mat.slice(0, 20) + '…' : mat;
    tabDefs.push({
      id: id,
      label: esc(label),
      iconHtml: '<span class="mini-dot ' + matCol(mat).cls + '"></span>',
      active: false
    });
  });

  const tabs = $('#res-tabs');
  const contents = $('#res-contents');
  const tabTemplate = getResTabTemplate();
  const contentTemplate = getResTabContentTemplate();
  tabs.html(tabDefs.map(function (tab) { return tabTemplate(tab); }).join(''));
  contents.html(tabDefs.map(function (tab) { return contentTemplate(tab); }).join(''));

  buildOutputTab('tab-output', outputRows, getToday());
  document.getElementById('tab-resumen').innerHTML = buildResumenTab(summaryRows, result.remaining || {});

  if (S.useOp) {
    buildBreakdownTab('tab-op_tab', summaryRows, 'opType', 'Tipo de Operador', OP_TYPES.map(function (item) {
      return { id: item.id, label: item.label, colorClass: item.colorClass };
    }));
  }

  if (S.useCls) {
    buildBreakdownTab('tab-cls_tab', summaryRows, 'descCl', 'Clase de Contrato', CLASS_ORDER.map(function (clase) {
      const meta = classMeta(clase);
      return { id: clase, label: meta.label, badgeClass: meta.cls, colorClass: 'bar-g' };
    }));
  }

  S.allMats.forEach(function (mat) {
    const id = 'mat_' + safeId(mat);
    buildMatTab('tab-' + id, mat, byMaterial[mat] || [], toNumber(S.limits[mat]), toNumber((result.remaining || {})[mat]));
  });

  updateActionButtons();
}

function switchTab(id, element) {
  $('#res-tabs .tab').removeClass('active');
  $('#res-contents .tc').removeClass('active');
  $(element).addClass('active');
  $('#tab-' + id).addClass('active');
}

function buildTotalsByMaterial(rows) {
  const totals = {};
  rows.forEach(function (row) {
    totals[row.material] = (totals[row.material] || 0) + row.cuposAssigned;
  });
  return Object.keys(totals).map(function (material) {
    return {
      material: material,
      color: matCol(material).cls,
      cupos: toLocaleNumber(totals[material]),
      tons: toLocaleNumber(totals[material] * 30)
    };
  });
}

function buildOutputGridRows(rows, fallbackDate) {
  return rows.map(function (row) {
    const operator = opMeta(row.opType);
    return Object.assign({}, row, {
      fechaLabel: row.fechaAsignada ? fmt(row.fechaAsignada, '/') : fallbackDate,
      opColorClass: operator.colorClass,
      matColClass: matCol(row.material).cls
    });
  });
}

function buildStatusCounts(rows) {
  return [
    { label: 'Total', value: rows.length, cls: '' },
    { label: 'Completo', value: rows.filter(function (row) { return row.status === 'completo'; }).length, cls: 'txt-green' },
    { label: 'Parcial', value: rows.filter(function (row) { return row.status === 'parcial'; }).length, cls: 'txt-org' },
    { label: 'CCPP', value: rows.filter(function (row) { return row.status === 'ccpp'; }).length, cls: 'txt-teal' },
    { label: 'Sin cupos', value: rows.filter(function (row) { return row.status === 'sin_cupos'; }).length, cls: 'txt-red' },
    { label: 'Bloq. filtros', value: rows.filter(function (row) { return row.status === 'bloq_op' || row.status === 'bloq_cls'; }).length, cls: 'txt-purple' }
  ];
}

function buildMaterialSummaryCards(rows, remaining) {
  return S.allMats.map(function (mat) {
    const limit = toNumber(S.limits[mat]);
    const rest = toNumber(remaining[mat]);
    const assigned = Math.max(0, limit - rest);
    const materialRows = rows.filter(function (row) { return row.material === mat; });
    return {
      material: mat,
      colorClass: matCol(mat).cls,
      limit: toLocaleNumber(limit),
      assigned: toLocaleNumber(assigned),
      remaining: toLocaleNumber(rest),
      covered: materialRows.filter(function (row) { return row.status === 'ccpp'; }).length,
      blocked: materialRows.filter(function (row) { return row.status === 'bloq_op' || row.status === 'bloq_cls'; }).length
    };
  });
}

function buildOutputTab(containerId, rows, today) {
  const container = document.getElementById(containerId);
  if (!container) {
    return;
  }
  const fallbackDate = fmt(today, '/');
  const totalList = buildTotalsByMaterial(rows);
  container.innerHTML = renderTemplate('tmpl-output-tab', {
    rows: rows,
    totals: totalList
  });

  if (!rows.length) {
    return;
  }

  renderDataGrid('grid-out', buildOutputGridRows(rows, fallbackDate), [
    { field: 'fechaLabel', title: 'FechaSugerida' },
    { field: 'cuposAssigned', title: 'CantidadDeCupos', attributes: { class: 'r' }, template: function (row) { return '<span class="big-num">' + row.cuposAssigned.toLocaleString('es-AR') + '</span>'; } },
    { field: 'numeroSAP', title: 'ContratoSAP', attributes: { class: 'mono bold' }, template: function (row) { return esc(row.numeroSAP); } },
    { field: 'prov', title: 'Proveedor', template: function (row) { return esc(row.prov); } },
    { field: 'cuit', title: 'CUIT', attributes: { class: 'mono' }, template: function (row) { return esc(row.cuit); } },
    { field: 'material', title: 'Material', template: function (row) { return '<span class="mini-dot ' + row.matColClass + '"></span>' + esc(row.material); } },
    { field: 'opLabel', title: 'Tipo Operador', template: function (row) { return '<span class="dot ' + row.opColorClass + '"></span> ' + esc(row.opLabel); } },
    { field: 'prioLabel', title: 'Clase', template: function (row) { return '<span class="bp ' + row.prioCls + '">' + esc(row.prioLabel) + '</span>'; } },
    { field: 'pricePt', title: 'Precio (USD/t)', attributes: { class: 'r' }, template: function (row) { return row.pricePt > 0 ? row.pricePt.toLocaleString('es-AR', { maximumFractionDigits: 1 }) : '—'; } },
    { field: 'isSust', title: '🌱', attributes: { class: 'text-center' }, template: function (row) { return row.isSust ? '🌱' : ''; } }
  ], { pageable: false });
}

function buildResumenTab(rows, remaining) {
  const cards = buildMaterialSummaryCards(rows, remaining);
  const stats = buildStatusCounts(rows);
  return renderTemplate('tmpl-resumen-tab', { cards: cards, stats: stats });
}

function buildBreakdownSummary(items, assignedRows, keyField, totalCupos) {
  const subsets = {};
  items.forEach(function (item) {
    subsets[item.id] = assignedRows.filter(function (row) { return row[keyField] === item.id; });
  });

  return items.map(function (item) {
    const subset = subsets[item.id];
    const cupos = subset.reduce(function (sum, row) { return sum + row.cuposAssigned; }, 0);
    return {
      label: item.label,
      badgeClass: item.badgeClass,
      colorClass: item.colorClass,
      count: subset.length,
      cupos: cupos,
      tons: cupos * 30,
      pct: cupos / totalCupos * 100,
      gridId: 'grid-breakdown-' + safeId(item.label) + '-' + safeId(item.id),
      rows: subset
    };
  });
}

function buildBreakdownTab(containerId, rows, keyField, title, items) {
  const container = document.getElementById(containerId);
  if (!container) {
    return;
  }
  const assignedRows = rows.filter(function (row) { return row.cuposAssigned > 0; });
  const totalCupos = assignedRows.reduce(function (sum, row) { return sum + row.cuposAssigned; }, 0) || 1;
  const summaryRows = buildBreakdownSummary(items, assignedRows, keyField, totalCupos);
  const groups = summaryRows.filter(function (row) { return row.rows.length; }).map(function (row) {
    return {
      label: row.label,
      count: row.count,
      gridId: row.gridId
    };
  });

  container.innerHTML = renderTemplate('tmpl-breakdown-tab', {
    title: title,
    summaryGridId: 'grid-breakdown-' + containerId,
    groups: groups
  });

  renderDataGrid('grid-breakdown-' + containerId, summaryRows, [
    { field: 'label', title: title, template: function (row) { return row.badgeClass ? '<span class="bp ' + row.badgeClass + '">' + esc(row.label) + '</span>' : '<span class="dot ' + row.colorClass + '"></span> ' + esc(row.label); } },
    { field: 'count', title: 'Contratos asignados', attributes: { class: 'r' } },
    { field: 'cupos', title: 'Cupos', attributes: { class: 'r bold' }, template: function (row) { return row.cupos.toLocaleString('es-AR'); } },
    { field: 'tons', title: 'Toneladas', attributes: { class: 'r' }, template: function (row) { return row.tons.toLocaleString('es-AR') + ' t'; } },
    { field: 'pct', title: '% del total', attributes: { class: 'r' }, template: function (row) { return row.pct.toFixed(1) + '%<span class="bar-bg"><span class="bar-fg ' + (row.colorClass || 'bar-g') + ' ' + pctWidthClass(row.pct) + '"></span></span>'; } }
  ], { sortable: false, pageable: false });

  summaryRows.forEach(function (row) {
    if (!row.rows.length) {
      return;
    }
    renderDataGrid(row.gridId, row.rows, [
      { field: 'numeroSAP', title: 'ContratoSAP', attributes: { class: 'mono' }, template: function (item) { return esc(item.numeroSAP); } },
      { field: 'prov', title: 'Proveedor', template: function (item) { return esc(item.prov); } },
      { field: 'cuit', title: 'CUIT', attributes: { class: 'mono' }, template: function (item) { return esc(item.cuit); } },
      { field: 'material', title: 'Material', template: function (item) { return esc(item.material); } },
      { field: 'cuposAssigned', title: 'Cupos', attributes: { class: 'r bold' }, template: function (item) { return item.cuposAssigned.toLocaleString('es-AR'); } }
    ], { pageable: false });
  });
}

function buildMatTab(containerId, material, rows, limit, remaining) {
  const container = document.getElementById(containerId);
  if (!container) {
    return;
  }
  const assigned = Math.max(0, limit - remaining);
  const gridId = 'grid-' + safeId(material);
  const ccppKg = toNumber(S.ccppByMat[material]);
  const ccppTon = Math.round((ccppKg / 1000) * 10) / 10;
  container.innerHTML = renderTemplate('tmpl-mat-tab', {
    limit: limit,
    assigned: assigned,
    remaining: remaining,
    ccppCupos: Math.ceil(ccppKg / (S.cupoKg || 30000)),
    ccppTon: ccppTon,
    rows: rows,
    gridId: gridId
  });

  const gridRows = rows.map(function (row) {
    const operator = opMeta(row.opType);
    return Object.assign({}, row, { opColorClass: operator.colorClass });
  });

  renderDataGrid(gridId, gridRows, [
    { field: 'numeroSAP', title: 'ContratoSAP', attributes: { class: 'mono bold' }, template: function (row) { return esc(row.numeroSAP); } },
    { field: 'prioLabel', title: 'Clase', template: function (row) { return '<span class="bp ' + row.prioCls + '">' + esc(row.prioLabel) + '</span>'; } },
    { field: 'opLabel', title: 'Tipo Operador', template: function (row) { return '<span class="dot ' + row.opColorClass + '"></span> ' + esc(row.opLabel); } },
    { field: 'prov', title: 'Proveedor', template: function (row) { return esc(row.prov); } },
    { field: 'cuit', title: 'CUIT', attributes: { class: 'mono' }, template: function (row) { return esc(row.cuit); } },
    { field: 'kg', title: 'KG Contrato', attributes: { class: 'r' }, template: function (row) { return (row.kg / 1000).toLocaleString('es-AR', { maximumFractionDigits: 1 }) + ' t'; } },
    { field: 'ccppKgUsed', title: 'CCPP Desc.', attributes: { class: 'r metric-org' }, template: function (row) { return row.ccppKgUsed > 0 ? (row.ccppKgUsed / 1000).toLocaleString('es-AR', { maximumFractionDigits: 1 }) + ' t' : '—'; } },
    { field: 'effKg', title: 'KG Efectivo', attributes: { class: 'r' }, template: function (row) { return (row.effKg / 1000).toLocaleString('es-AR', { maximumFractionDigits: 1 }) + ' t'; } },
    { field: 'cuposAssigned', title: 'Cupos Asig.', attributes: { class: 'r bold' }, template: function (row) { return '<span class="' + (row.cuposAssigned > 0 ? 'metric-pos' : 'metric-neg') + '">' + (row.cuposAssigned > 0 ? row.cuposAssigned.toLocaleString('es-AR') : '—') + '</span>'; } },
    { field: 'status', title: 'Estado', template: function (row) { return '<span class="bs s-' + row.status + '">' + (STATUS_LABELS[row.status] || row.status) + '</span>'; } },
    { field: 'cosecha', title: 'Cosecha', attributes: { class: 'mt' }, template: function (row) { return esc(row.cosecha); } },
    { field: 'fechaHasta', title: 'F.Hasta', attributes: { class: 'mt' }, template: function (row) { return fmt(row.fechaHasta, '.'); } },
    { field: 'pricePt', title: 'Precio (USD/t)', attributes: { class: 'r' }, template: function (row) { return row.pricePt > 0 ? row.pricePt.toLocaleString('es-AR', { maximumFractionDigits: 1 }) : '—'; } },
    { field: 'isSust', title: '🌱', attributes: { class: 'text-center' }, template: function (row) { return row.isSust ? '🌱' : ''; } }
  ], { pageable: false });
}

function copyOutput() {
  const sap = extractSapData();
  if (!sap.length) {
    showError('No hay resultado disponible para copiar.');
    return;
  }
  const lines = ['FechaSugerida\tCantidadDeCupos\tContratoSAP'].concat(sap.map(function (row) {
    return row.fechaSugerida + '\t' + row.cantidadDeCupos + '\t' + row.contratoSAP;
  }));
  navigator.clipboard.writeText(lines.join('\n'))
    .then(function () { showSuccess('✅ Tabla copiada. Pegala en Excel con Ctrl+V.'); })
    .catch(function () { showError('No se pudo copiar automáticamente.'); });
}

function exportExcel() {
  const sap = extractSapData();
  if (!sap.length) {
    showError('No hay resultado disponible para exportar.');
    return;
  }
  const today = getToday();
  const fileDate = String(today.getDate()).padStart(2, '0') + String(today.getMonth() + 1).padStart(2, '0') + today.getFullYear();
  const workbook = XLSX.utils.book_new();
  const rows = [['FechaSugerida', 'CantidadDeCupos', 'ContratoSAP']];
  sap.forEach(function (row) {
    rows.push([{ t: 's', v: row.fechaSugerida }, row.cantidadDeCupos, { t: 's', v: row.contratoSAP }]);
  });
  XLSX.utils.book_append_sheet(workbook, XLSX.utils.aoa_to_sheet(rows), 'Cupos');
  XLSX.writeFile(workbook, 'Cupos_SL_' + fileDate + '.xlsx');
}

function buildConfigFromUI() {
  return {
    plantaCodigo: 'SL',
    plantaNombre: 'Planta San Lorenzo',
    cupoKg: S.cupoKg,
    cuitMaxPct: S.cuitMaxPct,
    cosechasValidas: ['23-24', '24-25', '25-26'],
    clasesExcluidas: buildExcludedClasses(),
    limitesPredeterminadosPorMaterial: S.limits,
    preciosReferencia: S.prices,
    cuotasPorOperador: S.opPct,
    cuotasPorClase: S.clsPct
  };
}

function applyConfigToUi(config) {
  const normalized = camelizeKeys(config || {});
  S.config = normalized;
  S.cupoKg = toNumber(pick(normalized, 'cupoKg')) || 30000;
  const pct = pick(normalized, 'cuitMaxPct');
  S.cuitMaxPct = pct === undefined || pct === null || pct === '' ? 0.30 : Number(pct);
  S.prices = Object.assign({}, normalized.preciosReferencia || {});
  S.limits = Object.assign({}, normalized.limitesPredeterminadosPorMaterial || {}, S.limits);
  S.opPct = Object.assign(defaultOpPct(), normalized.cuotasPorOperador || {});
  S.clsPct = Object.assign(defaultClsPct(), normalized.cuotasPorClase || {});
  const excluded = Array.isArray(normalized.clasesExcluidas) ? normalized.clasesExcluidas : [];
  S.excludeFason = excluded.indexOf('MP-Fason') >= 0;
  S.useOp = Object.keys(S.opPct).some(function (key) { return toNumber(S.opPct[key]) > 0; });
  S.useCls = Object.keys(S.clsPct).some(function (key) { return toNumber(S.clsPct[key]) > 0; });
  syncStaticUi();
  if (S.contracts.length) {
    buildAll();
  }
}

async function loadConfig() {
  const response = await fetch(API_CONFIG_GET, { method: 'GET' });
  const data = await readJson(response);
  if (!response.ok) {
    throw new Error(apiMessage(data, 'No se pudo cargar la configuración.'));
  }
  applyConfigToUi(data);
}

async function saveConfig(showToast) {
  const response = await fetch(API_CONFIG_PUT, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(buildConfigFromUI())
  });
  const data = await readJson(response);
  if (!response.ok) {
    throw new Error(apiMessage(data, 'No se pudo guardar la configuración.'));
  }
  applyConfigToUi(Object.keys(data || {}).length ? data : buildConfigFromUI());
  if (showToast !== false) {
    showSuccess('Configuración guardada');
  }
}

async function procesarEnDataAgro() {
  const sapData = extractSapData();
  if (!sapData.length) {
    showError('No hay resultado para procesar.');
    return;
  }

  setBtnProcesar('loading');
  try {
    const response = await fetch(API_PROCESAR_DATAAGRO, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ sap: sapData })
    });
    const data = await readJson(response);
    if (!response.ok || !data.resultado) {
      throw new Error(apiMessage(data, 'Error al procesar en DataAgro.'));
    }
    showSuccess('✅ Cupos enviados a DataAgro correctamente');
  } catch (error) {
    showError(error.message || 'Error al procesar en DataAgro.');
  } finally {
    setBtnProcesar('idle');
  }
}
