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

function escapeJs(value) {
  return String(value || '').replace(/\\/g, '\\\\').replace(/'/g, "\\'");
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

function buildLimGrid() {
  const grid = document.getElementById('lim-grid');
  if (!grid) {
    return;
  }
  grid.innerHTML = '';
  const cupoKg = S.cupoKg || 30000;

  if (!S.multiDay || S.dayDates.length < 2) {
    S.allMats.forEach(function (mat) {
      const col = matCol(mat);
      const currentLimit = S.limits[mat] !== undefined ? S.limits[mat] : 0;
      const ccppKg = toNumber(S.ccppByMat[mat]);
      const ccppCupos = Math.ceil(ccppKg / cupoKg);
      const row = document.createElement('div');
      row.className = 'lim-row';
      row.innerHTML = `
        <div class="mat-nm"><span class="dot ${col.cls}"></span>${esc(mat)}</div>
        <div class="lim-inp">
          <label>Tope diario (cupos totales)</label>
          <input type="number" min="0" step="1" id="lim-${safeId(mat)}" value="${currentLimit}" oninput="S.limits['${escapeJs(mat)}']=parseInt(this.value,10)||0">
        </div>
        <div class="lim-ccpp">
          CCPP pendiente: <strong class="txt-org">${ccppCupos.toLocaleString('es-AR')} cupos</strong>
          <span class="d-block fs-11">${(ccppKg / 1000).toLocaleString('es-AR', { maximumFractionDigits: 0 })} t — descontado por CUIT</span>
        </div>`;
      grid.appendChild(row);
      S.limits[mat] = currentLimit;
    });
    return;
  }

  const header = document.createElement('div');
  header.className = 'lim-row lim-row-hdr ' + dayColsClass(S.dayDates.length);
  header.innerHTML = '<div>Material</div>' + S.dayDates.map(function (day, index) {
    return `<div class="text-center">Día ${index + 1}<br><span class="fw-400">${fmt(day, '/')}</span></div>`;
  }).join('') + '<div>CCPP pendiente</div>';
  grid.appendChild(header);

  S.allMats.forEach(function (mat) {
    const col = matCol(mat);
    const ccppKg = toNumber(S.ccppByMat[mat]);
    const ccppCupos = Math.ceil(ccppKg / cupoKg);
    if (!Array.isArray(S.dayLimits[mat])) {
      S.dayLimits[mat] = Array(S.dayDates.length).fill(0);
    }
    const row = document.createElement('div');
    row.className = 'lim-row ' + dayColsClass(S.dayDates.length);
    row.innerHTML = `
      <div class="mat-nm"><span class="dot ${col.cls}"></span>${esc(mat)}</div>
      ${S.dayDates.map(function (_, index) {
        const value = toNumber(S.dayLimits[mat][index]);
        return `<div class="lim-inp"><input class="w-100 input-center" type="number" min="0" step="1" id="lim-D${index}-${safeId(mat)}" value="${value}" oninput="if(!Array.isArray(S.dayLimits['${escapeJs(mat)}'])){S.dayLimits['${escapeJs(mat)}']=[];}S.dayLimits['${escapeJs(mat)}'][${index}]=parseInt(this.value,10)||0"></div>`;
      }).join('')}
      <div class="lim-ccpp">
        <strong class="txt-org">${ccppCupos.toLocaleString('es-AR')} cupos</strong>
        <span class="d-block fs-11">${(ccppKg / 1000).toLocaleString('es-AR', { maximumFractionDigits: 0 })} t</span>
      </div>`;
    grid.appendChild(row);
  });
}

function buildStatsUI() {
  const totalKg = S.contracts.reduce(function (sum, contract) { return sum + toNumber(contract.kg); }, 0) || 1;
  const statsOp = document.getElementById('stats-op');
  const statsCls = document.getElementById('stats-cls');

  if (statsOp) {
    statsOp.innerHTML = '';
    OP_TYPES.forEach(function (item) {
      const stat = S.stats.op[item.id] || { count: 0, kg: 0 };
      const pct = stat.kg / totalKg * 100;
      statsOp.insertAdjacentHTML('beforeend', `
        <tr>
          <td><span class="dot ${item.colorClass}"></span> ${esc(item.label)}</td>
          <td class="r">${stat.count}</td>
          <td class="r">${(stat.kg / 1e6).toFixed(1)}</td>
          <td class="r">${pct.toFixed(1)}%<span class="bar-bg"><span class="bar-fg ${item.colorClass} ${pctWidthClass(pct)}"></span></span></td>
        </tr>`);
    });
  }

  if (statsCls) {
    statsCls.innerHTML = '';
    CLASS_ORDER.forEach(function (clase) {
      const stat = S.stats.cls[clase] || { count: 0, kg: 0 };
      const meta = classMeta(clase);
      const pct = stat.kg / totalKg * 100;
      statsCls.insertAdjacentHTML('beforeend', `
        <tr>
          <td><span class="bp ${meta.cls}">${esc(meta.label)}</span></td>
          <td class="r">${stat.count}</td>
          <td class="r">${(stat.kg / 1e6).toFixed(1)}</td>
          <td class="r">${pct.toFixed(1)}%<span class="bar-bg"><span class="bar-fg bar-g ${pctWidthClass(pct)}"></span></span></td>
        </tr>`);
    });
  }
}

function buildPctInputs() {
  const pctOp = document.getElementById('pct-op');
  const pctCls = document.getElementById('pct-cls');

  if (pctOp) {
    pctOp.innerHTML = '';
    OP_TYPES.forEach(function (item) {
      const value = toNumber(S.opPct[item.id]);
      pctOp.insertAdjacentHTML('beforeend', `
        <div class="pct-row">
          <div class="pct-nm"><span class="dot ${item.colorClass}"></span>${esc(item.label)}</div>
          <input class="pct-inp" type="number" min="0" max="100" step="1" value="${value}" oninput="S.opPct['${item.id}']=parseInt(this.value,10)||0;updatePctTotal('op')">
          <span class="pct-sym">%</span>
        </div>`);
    });
  }

  if (pctCls) {
    pctCls.innerHTML = '';
    CLASS_ORDER.forEach(function (clase) {
      const meta = classMeta(clase);
      const value = toNumber(S.clsPct[clase]);
      pctCls.insertAdjacentHTML('beforeend', `
        <div class="pct-row">
          <div class="pct-nm"><span class="bp ${meta.cls}">${esc(meta.label)}</span></div>
          <input class="pct-inp" type="number" min="0" max="100" step="1" value="${value}" oninput="S.clsPct['${escapeJs(clase)}']=parseInt(this.value,10)||0;updatePctTotal('cls')">
          <span class="pct-sym">%</span>
        </div>`);
    });
  }

  updatePctTotal('op');
  updatePctTotal('cls');
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
  S.useOp = !!pick(document.getElementById('chk-op') || {}, 'checked');
  toggleHidden('pct-op-wrap', !S.useOp);
  toggleHidden('pct-op-off', S.useOp);
}

function onToggleCls() {
  S.useCls = !!pick(document.getElementById('chk-cls') || {}, 'checked');
  toggleHidden('pct-cls-wrap', !S.useCls);
  toggleHidden('pct-cls-off', S.useCls);
}

function toggleSection(sectionId, dividerId) {
  const section = document.getElementById(sectionId);
  const divider = document.getElementById(dividerId);
  if (!section || !divider) {
    return;
  }
  const isOpen = divider.classList.contains('open');
  divider.classList.toggle('open', !isOpen);
  section.classList.toggle('hidden', isOpen);
}

function buildDateFilterUI() {
  const container = document.getElementById('date-filter-ui');
  if (!container) {
    return;
  }
  const dates = S.stats.dates || {};
  container.innerHTML = `
    <div class="grid-2col mb-14">
      <div>
        <div class="section-caption-upper">RANGOS EN EL ARCHIVO</div>
        <div class="stats-box">
          <div class="sb-hdr sb-hdr-op">📅 Fechas de contratos CTO vigentes</div>
          <table><thead><tr><th>Campo</th><th class="r">Mínimo</th><th class="r">Máximo</th></tr></thead>
          <tbody>
            <tr><td>Fecha Desde</td><td class="r">${fmt(dates.fdMin, '.')}</td><td class="r">${fmt(dates.fdMax, '.')}</td></tr>
            <tr><td>Fecha Hasta</td><td class="r">${fmt(dates.fhMin, '.')}</td><td class="r">${fmt(dates.fhMax, '.')}</td></tr>
          </tbody></table>
        </div>
      </div>
      <div>
        <div class="toggle-row">
          <label class="toggle"><input type="checkbox" id="chk-date" onchange="onToggleDate()"><span class="slider"></span></label>
          <label for="chk-date">Filtrar contratos por rango de fechas</label>
        </div>
        <div id="date-filter-wrap" class="hidden">
          <div class="al al-p mb-10"><span class="ico">💡</span><div>Incluí solo contratos dentro de los rangos de fecha indicados. Dejá en blanco los campos que no querés restringir. Se aplica <em>además</em> del filtro automático de vigencia.</div></div>
          <div class="grid-gap-8">
            <div class="filter-title">Fecha Desde del contrato</div>
            <div class="grid-2col-gap8">
              <div><label class="field-label">No anterior a</label><input type="date" id="df-fdMin" class="input-ui w-100" value="${toInputDate(S.dateFilter.fdMin)}" onchange="onDateFilterChange()"></div>
              <div><label class="field-label">No posterior a</label><input type="date" id="df-fdMax" class="input-ui w-100" value="${toInputDate(S.dateFilter.fdMax)}" onchange="onDateFilterChange()"></div>
            </div>
            <div class="filter-title mt-8">Fecha Hasta del contrato</div>
            <div class="grid-2col-gap8">
              <div><label class="field-label">No anterior a</label><input type="date" id="df-fhMin" class="input-ui w-100" value="${toInputDate(S.dateFilter.fhMin)}" onchange="onDateFilterChange()"></div>
              <div><label class="field-label">No posterior a</label><input type="date" id="df-fhMax" class="input-ui w-100" value="${toInputDate(S.dateFilter.fhMax)}" onchange="onDateFilterChange()"></div>
            </div>
          </div>
        </div>
        <div id="date-filter-off" class="al al-w mt-8"><span class="ico">⚪</span>Sin filtro de fechas adicional. Se incluyen todos los contratos vigentes.</div>
      </div>
    </div>`;
  const checkbox = document.getElementById('chk-date');
  if (checkbox) {
    checkbox.checked = S.dateFilter.enabled;
  }
  onToggleDate();
}

function onToggleDate() {
  S.dateFilter.enabled = !!pick(document.getElementById('chk-date') || {}, 'checked');
  toggleHidden('date-filter-wrap', !S.dateFilter.enabled);
  toggleHidden('date-filter-off', S.dateFilter.enabled);
}

function onDateFilterChange() {
  S.dateFilter.fdMin = parseFlexibleDate(pick(document.getElementById('df-fdMin') || {}, 'value'));
  S.dateFilter.fdMax = parseFlexibleDate(pick(document.getElementById('df-fdMax') || {}, 'value'));
  S.dateFilter.fhMin = parseFlexibleDate(pick(document.getElementById('df-fhMin') || {}, 'value'));
  S.dateFilter.fhMax = parseFlexibleDate(pick(document.getElementById('df-fhMax') || {}, 'value'));
}

function buildPricesUI() {
  const container = document.getElementById('prices-sust-ui');
  if (!container) {
    return;
  }
  const sust = S.stats.sust || { count: 0, kg: 0 };
  const totalKg = S.contracts.reduce(function (sum, contract) { return sum + toNumber(contract.kg); }, 0) || 1;
  const sustPct = ((toNumber(sust.kg) / totalKg) * 100).toFixed(1);
  const materialInputs = S.allMats.map(function (mat) {
    const col = matCol(mat);
    const value = S.prices[mat] || '';
    return `
      <div class="lim-row">
        <div class="mat-nm"><span class="dot ${col.cls}"></span>${esc(mat)}</div>
        <div class="lim-inp">
          <label>Precio ref. (USD/tn)</label>
          <input type="number" min="0" step="0.01" id="price-${safeId(mat)}" value="${value}" placeholder="ej: 280" oninput="S.prices['${escapeJs(mat)}']=parseFloat(this.value)||0">
        </div>
        <div class="lim-ccpp">Contratos Fijo/Hijo con precio ≥ ref.<br><span class="fs-11">se priorizan dentro de su clase</span></div>
      </div>`;
  }).join('');

  container.innerHTML = `
    <div class="grid-2col mb-14">
      <div>
        <div class="section-caption-upper">CONTRATOS SUSTENTABLES EN EL ARCHIVO</div>
        <div class="stats-box">
          <div class="sb-hdr sb-hdr-sust">🌱 Sust. / EPA</div>
          <table><thead><tr><th>Indicador</th><th class="r">Valor</th></tr></thead>
          <tbody>
            <tr><td>Contratos con Sust.=X o EPA=X</td><td class="r"><strong class="txt-green">${toNumber(sust.count)}</strong></td></tr>
            <tr><td>Kg sustentables (M)</td><td class="r">${(toNumber(sust.kg) / 1e6).toFixed(2)} M kg</td></tr>
            <tr><td>% del total kg</td><td class="r">${sustPct}%</td></tr>
          </tbody></table>
        </div>
      </div>
      <div>
        <div class="toggle-row">
          <label class="toggle"><input type="checkbox" id="chk-sust" onchange="S.sustainFirst=this.checked"><span class="slider"></span></label>
          <label for="chk-sust">🌱 Priorizar contratos sustentables</label>
        </div>
        <div class="al al-t mt-8"><span class="ico">ℹ️</span><div>Dentro de cada clase de prioridad, los contratos con <strong>Sust.=X</strong> o <strong>EPA=X</strong> se asignarán antes que los no sustentables.</div></div>
      </div>
    </div>
    <div>
      <div class="toggle-row">
        <label class="toggle"><input type="checkbox" id="chk-prices" onchange="onTogglePrices()"><span class="slider"></span></label>
        <label for="chk-prices">💰 Priorizar contratos Fijo/Hijo por precio de referencia</label>
      </div>
      <div id="prices-wrap" class="hidden">
        <div class="al al-p mb-10"><span class="ico">💡</span><div>Los contratos <strong>Fijo</strong> y <strong>Contrato Hijo</strong> con precio ≥ precio de referencia se asignarán antes que los de precio menor o sin precio. Si no se ingresa precio de referencia, todos los contratos con precio se priorizan sobre los sin precio.</div></div>
        <div class="lim-grid">${materialInputs}</div>
      </div>
      <div id="prices-off" class="al al-w mt-8"><span class="ico">⚪</span>Sin priorización por precio de referencia (contratos Fijo/Hijo con precio se asignan antes que sin precio). Activá para definir un umbral por material.</div>
    </div>`;

  const sustain = document.getElementById('chk-sust');
  const prices = document.getElementById('chk-prices');
  if (sustain) { sustain.checked = S.sustainFirst; }
  if (prices) { prices.checked = S.pricesEnabled; }
  onTogglePrices();
}

function onTogglePrices() {
  S.pricesEnabled = !!pick(document.getElementById('chk-prices') || {}, 'checked');
  toggleHidden('prices-wrap', !S.pricesEnabled);
  toggleHidden('prices-off', S.pricesEnabled);
}

function updateExclBadge(which) {
  if (which === 'fason') {
    S.excludeFason = !!pick(document.getElementById('chk-excl-fason') || {}, 'checked');
    const label = document.getElementById('lbl-excl-fason');
    if (label) {
      label.classList.toggle('excl-on-red', S.excludeFason);
    }
    return;
  }
  S.excludeAgCompra = !!pick(document.getElementById('chk-excl-agcompra') || {}, 'checked');
  const label = document.getElementById('lbl-excl-agcompra');
  if (label) {
    label.classList.toggle('excl-on-purple', S.excludeAgCompra);
  }
}

function onToggleMultiDay() {
  S.multiDay = !!pick(document.getElementById('chk-multiday') || {}, 'checked');
  toggleHidden('multiday-config', !S.multiDay);
  updateDayDates();
  buildLimGrid();
}

function onDayCountChange() {
  const value = parseInt(pick(document.getElementById('day-count') || {}, 'value'), 10) || 2;
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
    preview.innerHTML = S.dayDates.map(function (day, index) {
      return `<span class="day-badge">Día ${index + 1}: ${fmt(day, '/')}</span>`;
    }).join('');
  }
}

function readSingleDayLimits() {
  const limits = {};
  S.allMats.forEach(function (mat) {
    const input = document.getElementById('lim-' + safeId(mat));
    limits[mat] = input ? (parseInt(input.value, 10) || 0) : toNumber(S.limits[mat]);
  });
  S.limits = limits;
  return limits;
}

function readMultiDayLimits() {
  const limitsByDay = [];
  const totalByMaterial = {};
  S.allMats.forEach(function (mat) { totalByMaterial[mat] = 0; });
  S.dayDates.forEach(function (day, dayIndex) {
    const dayLimits = {};
    S.allMats.forEach(function (mat) {
      const input = document.getElementById('lim-D' + dayIndex + '-' + safeId(mat));
      const value = input ? (parseInt(input.value, 10) || 0) : toNumber((S.dayLimits[mat] || [])[dayIndex]);
      if (!Array.isArray(S.dayLimits[mat])) {
        S.dayLimits[mat] = [];
      }
      S.dayLimits[mat][dayIndex] = value;
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

function buildSingleDayRequest() {
  return {
    fecha: toInputDate(getToday()),
    contratos: S.contracts,
    ccppByCuitMat: S.ccppByCuitMat,
    limitesPorMaterial: readSingleDayLimits(),
    configuracion: buildDistributionConfig()
  };
}

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

  const tabs = document.getElementById('res-tabs');
  const contents = document.getElementById('res-contents');
  tabs.innerHTML = '';
  contents.innerHTML = '';

  addTab(tabs, contents, 'output', '📋 Cupos a asignar', true);
  document.getElementById('tab-output').innerHTML = buildOutputTab(outputRows, getToday());

  addTab(tabs, contents, 'resumen', '📊 Resumen');
  document.getElementById('tab-resumen').innerHTML = buildResumenTab(summaryRows, result.remaining || {});

  if (S.useOp) {
    addTab(tabs, contents, 'op_tab', '🏢 Por tipo operador');
    document.getElementById('tab-op_tab').innerHTML = buildBreakdownTab(summaryRows, 'opType', 'Tipo de Operador', OP_TYPES.map(function (item) {
      return { id: item.id, label: item.label, colorClass: item.colorClass };
    }));
  }

  if (S.useCls) {
    addTab(tabs, contents, 'cls_tab', '📋 Por clase contrato');
    document.getElementById('tab-cls_tab').innerHTML = buildBreakdownTab(summaryRows, 'descCl', 'Clase de Contrato', CLASS_ORDER.map(function (clase) {
      const meta = classMeta(clase);
      return { id: clase, label: meta.label, badgeClass: meta.cls, colorClass: 'bar-g' };
    }));
  }

  S.allMats.forEach(function (mat) {
    const id = 'mat_' + safeId(mat);
    const tab = document.createElement('div');
    tab.className = 'tab';
    tab.onclick = function () { switchTab(id, tab); };
    tab.innerHTML = `<span class="mini-dot ${matCol(mat).cls}"></span>${esc(mat.length > 20 ? mat.slice(0, 20) + '…' : mat)}`;
    tabs.appendChild(tab);

    const content = document.createElement('div');
    content.className = 'tc';
    content.id = 'tab-' + id;
    content.innerHTML = buildMatTab(mat, byMaterial[mat] || [], toNumber(S.limits[mat]), toNumber((result.remaining || {})[mat]));
    contents.appendChild(content);
  });

  updateActionButtons();
}

function addTab(tabs, contents, id, label, active) {
  tabs.insertAdjacentHTML('beforeend', `<div class="tab${active ? ' active' : ''}" onclick="switchTab('${id}',this)">${label}</div>`);
  const content = document.createElement('div');
  content.className = 'tc' + (active ? ' active' : '');
  content.id = 'tab-' + id;
  contents.appendChild(content);
}

function switchTab(id, element) {
  document.querySelectorAll('#res-tabs .tab').forEach(function (tab) { tab.classList.remove('active'); });
  document.querySelectorAll('#res-contents .tc').forEach(function (content) { content.classList.remove('active'); });
  element.classList.add('active');
  const target = document.getElementById('tab-' + id);
  if (target) {
    target.classList.add('active');
  }
}

function buildOutputTab(rows, today) {
  let html = '<div class="al al-t"><span class="ico">✅</span><div>Tabla para cargar en SAP: <strong>FechaSugerida / CantidadDeCupos / ContratoSAP</strong>. Solo contratos con cupos asignados.</div></div>';
  html += `<div class="sb-row"><input type="text" placeholder="Buscar contrato, CUIT, proveedor…" oninput="filterTable('tbl-out',this.value)"><span class="search-meta">${rows.length} líneas</span></div>`;
  if (!rows.length) {
    return html + '<div class="al al-w"><span class="ico">⚠️</span>No hay contratos con cupos asignados.</div>';
  }

  const fallbackDate = fmt(today, '/');
  html += '<div class="tw"><table id="tbl-out" class="out-table"><thead><tr><th>FechaSugerida</th><th class="r">CantidadDeCupos</th><th>ContratoSAP</th><th>Proveedor</th><th>CUIT</th><th>Material</th><th>Tipo Operador</th><th>Clase</th><th class="r">Precio (USD/t)</th><th class="text-center">🌱</th></tr></thead><tbody>';
  rows.forEach(function (row) {
    const operator = opMeta(row.opType);
    html += `
      <tr>
        <td>${row.fechaAsignada ? fmt(row.fechaAsignada, '/') : fallbackDate}</td>
        <td class="r"><span class="big-num">${row.cuposAssigned.toLocaleString('es-AR')}</span></td>
        <td class="mono bold">${esc(row.numeroSAP)}</td>
        <td>${esc(row.prov)}</td>
        <td class="mono">${esc(row.cuit)}</td>
        <td><span class="mini-dot ${matCol(row.material).cls}"></span>${esc(row.material)}</td>
        <td><span class="dot ${operator.colorClass}"></span> ${esc(row.opLabel)}</td>
        <td><span class="bp ${row.prioCls}">${esc(row.prioLabel)}</span></td>
        <td class="r">${row.pricePt > 0 ? row.pricePt.toLocaleString('es-AR', { maximumFractionDigits: 1 }) : '—'}</td>
        <td class="text-center">${row.isSust ? '🌱' : ''}</td>
      </tr>`;
  });
  html += '</tbody></table></div>';

  const totals = {};
  rows.forEach(function (row) { totals[row.material] = (totals[row.material] || 0) + row.cuposAssigned; });
  html += '<div class="totals-box"><strong>Totales:</strong>';
  Object.keys(totals).forEach(function (material) {
    html += `<span><span class="mini-dot-sm ${matCol(material).cls}"></span>${esc(material)}: <strong>${totals[material].toLocaleString('es-AR')} cupos</strong> (${(totals[material] * 30).toLocaleString('es-AR')} t)</span>`;
  });
  html += '</div>';
  return html;
}

function buildResumenTab(rows, remaining) {
  let html = '<div class="grid-auto-265">';
  S.allMats.forEach(function (mat) {
    const limit = toNumber(S.limits[mat]);
    const rest = toNumber(remaining[mat]);
    const assigned = Math.max(0, limit - rest);
    const materialRows = rows.filter(function (row) { return row.material === mat; });
    const covered = materialRows.filter(function (row) { return row.status === 'ccpp'; }).length;
    const blocked = materialRows.filter(function (row) { return row.status === 'bloq_op' || row.status === 'bloq_cls'; }).length;
    html += `
      <div class="summary-card">
        <div class="summary-card-hdr ${matCol(mat).cls}">${esc(mat)}</div>
        <div class="summary-card-body">
          <table class="summary-table">
            <tr><td class="summary-label">Tope diario</td><td class="text-right summary-val">${limit.toLocaleString('es-AR')} cupos</td></tr>
            <tr><td class="summary-label">Cupos asignados</td><td class="text-right summary-val txt-teal">${assigned.toLocaleString('es-AR')}</td></tr>
            <tr><td class="summary-label">Sin asignar</td><td class="text-right ${rest > 0 ? 'txt-gold' : 'summary-val-rem'}">${rest.toLocaleString('es-AR')}</td></tr>
            <tr><td class="summary-label">Cubiertos por CCPP</td><td class="text-right txt-teal">${covered}</td></tr>
            ${blocked > 0 ? `<tr><td class="summary-label">Bloqueados por filtros</td><td class="text-right txt-purple">${blocked}</td></tr>` : ''}
          </table>
        </div>
      </div>`;
  });
  html += '</div>';

  const total = rows.length;
  const complete = rows.filter(function (row) { return row.status === 'completo'; }).length;
  const partial = rows.filter(function (row) { return row.status === 'parcial'; }).length;
  const ccpp = rows.filter(function (row) { return row.status === 'ccpp'; }).length;
  const noCupos = rows.filter(function (row) { return row.status === 'sin_cupos'; }).length;
  const blockedCount = rows.filter(function (row) { return row.status === 'bloq_op' || row.status === 'bloq_cls'; }).length;

  html += '<div class="summary-box"><div class="summary-box-title">📈 Estadísticas de contratos</div><div class="grid-auto-140">';
  [
    [total, 'Total', ''],
    [complete, 'Completo', 'txt-green'],
    [partial, 'Parcial', 'txt-org'],
    [ccpp, 'CCPP', 'txt-teal'],
    [noCupos, 'Sin cupos', 'txt-red'],
    [blockedCount, 'Bloq. filtros', 'txt-purple']
  ].forEach(function (item) {
    html += `<div class="summary-stat"><div class="summary-stat-value ${item[2]}">${item[0]}</div><div class="summary-stat-label">${item[1]}</div></div>`;
  });
  html += '</div></div>';
  return html;
}

function buildBreakdownTab(rows, keyField, title, items) {
  const assignedRows = rows.filter(function (row) { return row.cuposAssigned > 0; });
  const totalCupos = assignedRows.reduce(function (sum, row) { return sum + row.cuposAssigned; }, 0) || 1;
  let html = `<h3 class="breakdown-title">📊 Distribución real de cupos asignados por ${esc(title)}</h3>`;
  html += `<div class="tw mb-14"><table><thead><tr><th>${esc(title)}</th><th class="r">Contratos asignados</th><th class="r">Cupos</th><th class="r">Toneladas</th><th class="r">% del total</th></tr></thead><tbody>`;
  items.forEach(function (item) {
    const subset = assignedRows.filter(function (row) { return row[keyField] === item.id; });
    const cupos = subset.reduce(function (sum, row) { return sum + row.cuposAssigned; }, 0);
    const pct = cupos / totalCupos * 100;
    html += `
      <tr>
        <td>${item.badgeClass ? `<span class="bp ${item.badgeClass}">${esc(item.label)}</span>` : `<span class="dot ${item.colorClass}"></span> ${esc(item.label)}`}</td>
        <td class="r">${subset.length}</td>
        <td class="r bold">${cupos.toLocaleString('es-AR')}</td>
        <td class="r">${(cupos * 30).toLocaleString('es-AR')} t</td>
        <td class="r">${pct.toFixed(1)}%<span class="bar-bg"><span class="bar-fg ${item.colorClass || 'bar-g'} ${pctWidthClass(pct)}"></span></span></td>
      </tr>`;
  });
  html += '</tbody></table></div>';
  items.forEach(function (item) {
    const subset = assignedRows.filter(function (row) { return row[keyField] === item.id; });
    if (!subset.length) {
      return;
    }
    html += `<h4 class="breakdown-subtitle">${esc(item.label)} — ${subset.length} contratos</h4>`;
    html += '<div class="tw mb-14"><table><thead><tr><th>ContratoSAP</th><th>Proveedor</th><th>CUIT</th><th>Material</th><th class="r">Cupos</th></tr></thead><tbody>';
    subset.forEach(function (row) {
      html += `<tr><td class="mono">${esc(row.numeroSAP)}</td><td>${esc(row.prov)}</td><td class="mono">${esc(row.cuit)}</td><td>${esc(row.material)}</td><td class="r bold">${row.cuposAssigned.toLocaleString('es-AR')}</td></tr>`;
    });
    html += '</tbody></table></div>';
  });
  return html;
}

function buildMatTab(material, rows, limit, remaining) {
  const assigned = Math.max(0, limit - remaining);
  const tableId = 'tbl-' + safeId(material);
  let html = `
    <div class="sg">
      <div class="sc sc-g"><div class="lb">Tope diario</div><div class="vl">${limit.toLocaleString('es-AR')}</div><div class="sb">cupos</div></div>
      <div class="sc sc-t"><div class="lb">Asignados</div><div class="vl">${assigned.toLocaleString('es-AR')}</div><div class="sb">${(assigned * 30).toLocaleString('es-AR')} t</div></div>
      <div class="sc sc-gl"><div class="lb">Sin asignar</div><div class="vl">${remaining.toLocaleString('es-AR')}</div><div class="sb">cupos remanentes</div></div>
      <div class="sc sc-o"><div class="lb">CCPP material</div><div class="vl">${Math.ceil(toNumber(S.ccppByMat[material]) / (S.cupoKg || 30000)).toLocaleString('es-AR')}</div><div class="sb">${(toNumber(S.ccppByMat[material]) / 1000).toLocaleString('es-AR', { maximumFractionDigits: 0 })} t</div></div>
    </div>`;
  html += `<div class="sb-row"><input type="text" placeholder="Buscar…" oninput="filterTable('${tableId}',this.value)"><span class="search-meta">${rows.length} contratos</span></div>`;
  html += `<div class="tw"><table id="${tableId}"><thead><tr><th>ContratoSAP</th><th>Clase</th><th>Tipo Operador</th><th>Proveedor</th><th>CUIT</th><th class="r">KG Contrato</th><th class="r">CCPP Desc.</th><th class="r">KG Efectivo</th><th class="r">Cupos Asig.</th><th>Estado</th><th>Cosecha</th><th>F.Hasta</th><th class="r">Precio (USD/t)</th><th class="text-center">🌱</th></tr></thead><tbody>`;
  rows.forEach(function (row) {
    const operator = opMeta(row.opType);
    html += `
      <tr>
        <td class="mono bold">${esc(row.numeroSAP)}</td>
        <td><span class="bp ${row.prioCls}">${esc(row.prioLabel)}</span></td>
        <td><span class="dot ${operator.colorClass}"></span> ${esc(row.opLabel)}</td>
        <td>${esc(row.prov)}</td>
        <td class="mono">${esc(row.cuit)}</td>
        <td class="r">${(row.kg / 1000).toLocaleString('es-AR', { maximumFractionDigits: 1 })} t</td>
        <td class="r metric-org">${row.ccppKgUsed > 0 ? (row.ccppKgUsed / 1000).toLocaleString('es-AR', { maximumFractionDigits: 1 }) + ' t' : '—'}</td>
        <td class="r">${(row.effKg / 1000).toLocaleString('es-AR', { maximumFractionDigits: 1 })} t</td>
        <td class="r bold ${row.cuposAssigned > 0 ? 'metric-pos' : 'metric-neg'}">${row.cuposAssigned > 0 ? row.cuposAssigned.toLocaleString('es-AR') : '—'}</td>
        <td><span class="bs s-${row.status}">${STATUS_LABELS[row.status] || row.status}</span></td>
        <td class="mt">${esc(row.cosecha)}</td>
        <td class="mt">${fmt(row.fechaHasta, '.')}</td>
        <td class="r">${row.pricePt > 0 ? row.pricePt.toLocaleString('es-AR', { maximumFractionDigits: 1 }) : '—'}</td>
        <td class="text-center">${row.isSust ? '🌱' : ''}</td>
      </tr>`;
  });
  html += '</tbody></table></div>';
  return html;
}

function filterTable(id, query) {
  const table = document.getElementById(id);
  if (!table) {
    return;
  }
  const normalizedQuery = String(query || '').toLowerCase();
  table.querySelectorAll('tbody tr').forEach(function (row) {
    row.classList.toggle('row-hidden', !row.textContent.toLowerCase().includes(normalizedQuery));
  });
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
