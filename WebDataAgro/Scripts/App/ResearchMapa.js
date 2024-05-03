import PhotoSwipeLightbox from 'https://unpkg.com/photoswipe/dist/photoswipe-lightbox.esm.js';
var markers;
var map = L.map('map'
    , {
        fullscreenControl: true,
        fullscreenControlOptions: {
            position: 'topleft'
        }
    }
).setView([-38.678907, -61.104765], 5);
var addressPoints = [{}];

const filterButton = document.getElementById("filter-button");
const filterForm = document.getElementById("filter-form");
var maxClusterRadius = 80;

const LeafIcon = L.Icon.extend({
    options: {
        iconSize: [28, 45],
        iconAnchor: [15, 40],
        popupAnchor: [-3, -35]
    }
});

const maizIcon = new LeafIcon({ iconUrl: 'maiz.png' });
const girasolIcon = new LeafIcon({ iconUrl: 'girasol.png' });
const trigoIcon = new LeafIcon({ iconUrl: 'trigo.png' });
const sojaIcon = new LeafIcon({ iconUrl: 'soja.png' });

L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

L.control.scale().addTo(map);

L.Control.Watermark = L.Control.extend({
    onAdd: function (map) {
        var div = L.DomUtil.create('div');
        var divAMover = $("#filter-panel");
        divAMover.appendTo(div);
        return div;
    },
    onRemove: function (map) {

    }
});

L.control.watermark = function (opts) {
    return new L.Control.Watermark(opts);
}
L.control.watermark({ position: 'topright' }).addTo(map);

markers = L.markerClusterGroup({
    maxClusterRadius: maxClusterRadius
});

$(document).ready(function () {
    buscadorPorId();
    busquedaFiltrada();
});

//mostrar/ocultar el panel de filtros
filterButton.addEventListener("click", () => {
    if (filterForm.style.display === "none") {
        filterForm.style.display = "block";
    } else {
        filterForm.style.display = "none";
    }
});

document.getElementById("GranoId").addEventListener("change", function () {
    if (document.getElementById("GranoId").value == "") {
        document.getElementById("CampanaId").value = "";
    } else {
        var result = MSExecuteOnServer("/CompraNet/TraerCampanaActualMaterial", { MaterialId: document.getElementById("GranoId").value });
        document.getElementById("CampanaId").value = result;
    }
    busquedaFiltrada();
});
document.getElementById("CampanaId").addEventListener("change", function () {
    busquedaFiltrada();
});

//document.getElementById("ResearchId").addEventListener("change", function () {
//    busquedaFiltrada();
//});

document.getElementById("AgrupadoId").addEventListener("change", function () {
    busquedaFiltrada();
});

function busquedaFiltrada() {
    var granoId = Number(document.getElementById("GranoId").value);
    var campañaId = Number(document.getElementById("CampanaId").value);
    var id = Number(document.getElementById("ResearchId").value);
    if (granoId == 0 || granoId == NaN) {
        granoId = null;
    }
    if (campañaId == 0 || campañaId == NaN) {
        campañaId = null;
    }
    if (id == 0 || id == NaN) {
        id = null;
    }
    var agrupadoId = document.getElementById("AgrupadoId");
    if (agrupadoId.checked) {
        maxClusterRadius = 80;
    } else {
        maxClusterRadius = 0;
    }
    var filtro = {
        "take": 2147483647,
        "filter": {
            "field": null,
            "value": {},
            "logic": "and",
            "operator": null,
            "filters": [
                {
                    "field": null,
                    "value": {},
                    "logic": "or",
                    "operator": null,
                    "filters": [
                        {
                            "field": "Eliminado",
                            "value": false,
                            "logic": null,
                            "operator": "eq",
                            "filters": {}
                        }
                    ]
                }
            ]
        },
        "pageSize": 2147483647
    };

    if (granoId != null) {
        filtro.filter.filters.push(
            {
                "field": null,
                "value": {},
                "logic": "or",
                "operator": null,
                "filters": [
                    {
                        "field": "MaterialId",
                        "value": granoId,
                        "logic": null,
                        "operator": "eq",
                        "filters": {}
                    }
                ]
            });
    }
    if (campañaId != null) {
        filtro.filter.filters.push(
            {
                "field": null,
                "value": {},
                "logic": "or",
                "operator": null,
                "filters": [
                    {
                        "field": "CampañaId",
                        "value": campañaId,
                        "logic": null,
                        "operator": "eq",
                        "filters": {}
                    }
                ]
            });
    }
    if (id != null) {
        filtro.filter.filters.push(
            {
                "field": null,
                "value": {},
                "logic": "or",
                "operator": null,
                "filters": [
                    {
                        "field": "Id",
                        "value": id,
                        "logic": null,
                        "operator": "eq",
                        "filters": {}
                    }
                ]
            });
    }


    BlockUi('Cargando...');
    addressPoints = [];
    var result = MSExecuteOnServer("/ResearchReporte/BuscaDatosTabla", filtro);
    $.unblockUI();
    result.Data.forEach(x => addressPoints.push({
        tipoCarga: x.TipoCarga,
        grano: x.Material,
        antecesor: x.MaterialAntecesor,
        campania: x.Campaña,
        estadoFenologico: x.Estadio,
        condicion: x.Condicion,
        humedad: x.HumedadSuelo,
        comentarios: x.Comentarios,
        provincia: x.Provincia,
        partido: x.Partido,
        localidad: x.Localidad,
        rendimiento: x.Rendimiento,
        rendimientoCalculado: x.RendimientoCalculado,
        tipoMuestra1: x.TipoMuestraUno,
        medida1: x.MedidasUno,
        promedio1: x.PromedioMuestraUno,
        tipoMuestra2: x.TipoMuestraDos,
        medida2: x.MedidasDos,
        promedio2: x.PromedioMuestraDos,
        tipoMuestra3: x.TipoMuestraTres,
        medida3: x.MedidasTres,
        promedio3: x.PromedioMuestraTres,
        promedioGranosVaina: x.PromedioGranosVaina,
        lat: x.Latitud,
        lng: x.Longitud,
        id: x.Id,
        idPowerApp: x.IdPowerApp,
        imgs: x.Adjuntos.map((adjunto) => {
            return adjunto.Path;
        })
        //"https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg",
        //"https://fastly.picsum.photos/id/866/2000/3000.jpg?hmac=_X5OfUFRDyrhMZM4Z43W_pCrWO3_3tFYaAa_MWe23f0"

    }));

    updateMarkers();
}

function updateMarkers() {
    map.removeLayer(markers);

    markers = L.markerClusterGroup({
        maxClusterRadius: maxClusterRadius
    });
    for (var i = 0; i < addressPoints.length; i++) {
        var a = addressPoints[i];
        //if (a.grano == "Soja") {
        //    a.icon = sojaIcon;
        //} else if (a.grano == "Maiz") {
        //    a.icon = maizIcon;
        //} else if (a.grano == "Trigo") {
        //    a.icon = trigoIcon;
        //} else if (a.grano == "Girasol") {
        //    a.icon = girasolIcon;
        //}

        var marker = L.marker(new L.LatLng(a.lat, a.lng), a);
        marker.bindPopup(cargarInformacionPopup);
        markers.addLayer(marker);
    }

    map.addLayer(markers);

    //L.marker([51.5, -0.09]).addTo(map).bindPopup("I am a green leaf.");
    //L.marker([51.495, -0.083]).addTo(map).bindPopup("I am a red leaf.");
    //L.marker([51.49, -0.1]).addTo(map).bindPopup("I am an orange leaf.");

}

// Función para cargar una imagen y obtener sus dimensiones
function loadImageDimensions(src) {
    return new Promise((resolve, reject) => {
        const imgElement = new Image();
        imgElement.src = src;
        imgElement.onload = function () {
            const width = imgElement.width;
            const height = imgElement.height;
            resolve({ src: src, width: width, height: height });
        };
        imgElement.onerror = function () {
            reject("Error al cargar la imagen: " + src);
        };
    });
}

// Función para cargar la información del popup
function cargarInformacionPopup(popup) {
    if (document.getElementById("info-carousel") == null) {
        var elemDiv = document.createElement('div');
        elemDiv.setAttribute("id", "info-carousel");
        document.body.appendChild(elemDiv);
    }

    document.getElementById("info-carousel").innerHTML = ''
        + '<div class="info-carousel" id="info-carousel" class="leaflet-popup-content">'
        + '<div id="carousel-images"> </div>'
        + '<div class="row">'
        + '<div class="col-md-12"><span class="bold-text">ID:</span> <span id="id"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Grano:</span> <span id="grano"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Campaña:</span> <span id="campania"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Tipo de carga:</span> <span id="tipoCarga"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Antecesor:</span> <span id="antecesor"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Ubicación:</span> <span id="ubicacion"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Estado Fenológico:</span> <span id="estadoFenologico"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Condición del Cultivo:</span> <span id="condicion"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Humedad del Suelo:</span> <span id="humedad"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Rendimiento:</span> <span id="rendimiento"></span></div>'
        //+ '<div class="col-md-12"><span class="bold-text">Rendimiento Calculado:</span> <span id="rendimientoCalculado"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Comentarios:</span> <span id="comentarios"></span></div>'
        + '<div class="col-md-12"><span class="bold-text" id="tipoMuestra1"></span> <span id="medida1"></span></div>'
        + '<div class="col-md-12"><span class="bold-text" id="tipoMuestra2"></span> <span id="medida2"></span></div>'
        + '<div class="col-md-12"><span class="bold-text" id="tipoMuestra3"></span> <span id="medida3"></span></div>'
        + '<div class="col-md-12"><span class="bold-text" id="granosVaina"></span> <span id="promedioGranosVaina"></span></div>'
        + '</div>'
        + '</div>';

    document.getElementById("tipoCarga").innerHTML = popup.options.tipoCarga;
    document.getElementById("id").innerHTML = popup.options.id;
    document.getElementById("grano").innerHTML = popup.options.grano;
    document.getElementById("antecesor").innerHTML = popup.options.antecesor;
    document.getElementById("campania").innerHTML = popup.options.campania;
    document.getElementById("estadoFenologico").innerHTML = popup.options.estadoFenologico;
    document.getElementById("condicion").innerHTML = popup.options.condicion;
    document.getElementById("humedad").innerHTML = popup.options.humedad;
    document.getElementById("comentarios").innerHTML = popup.options.comentarios;
    document.getElementById("ubicacion").innerHTML = `${popup.options.provincia == null ? "" : (popup.options.provincia + ", ")} ${popup.options.partido == null ? "" : (popup.options.partido + ", ")} ${popup.options.localidad == null ? "" : popup.options.localidad}`;
    document.getElementById("rendimiento").innerHTML = popup.options.rendimiento;
    //document.getElementById("rendimientoCalculado").innerHTML = popup.options.rendimientoCalculado;

    if (popup.options.tipoMuestra1 != null && popup.options.tipoMuestra1 != "") {
        document.getElementById("tipoMuestra1").innerHTML = `${popup.options.tipoMuestra1}: `;
        document.getElementById("medida1").innerHTML = `${popup.options.promedio1} (${popup.options.medida1})`;
    }
    if (popup.options.tipoMuestra2 != null && popup.options.tipoMuestra2 != "") {
        document.getElementById("tipoMuestra2").innerHTML = `${popup.options.tipoMuestra2}: `;
        document.getElementById("medida2").innerHTML = `${popup.options.promedio2} (${popup.options.medida2})`;
    }
    if (popup.options.tipoMuestra3 != null && popup.options.tipoMuestra3 != "") {
        document.getElementById("tipoMuestra3").innerHTML = `${popup.options.tipoMuestra3}: `;
        document.getElementById("medida3").innerHTML = `${popup.options.promedio3} (${popup.options.medida3})`;
    }
    if (popup.options.promedioGranosVaina != null && popup.options.promedioGranosVaina != "") {
        document.getElementById("granosVaina").innerHTML = "Promedio de granos por vaina:";
        document.getElementById("promedioGranosVaina").innerHTML = popup.options.promedioGranosVaina;
    }

    var carouselImages = document.getElementById('carousel-images');
    carouselImages.innerHTML = '<div class="pswp-gallery" id="my-gallery"></div>';

    popup.options.imgs.forEach(function (link, index) {

        loadImageDimensions(link)
            .then((imageDimensions) => {
                var a = '<a href="' + link + '" data-pswp-width="' + imageDimensions.width + '" data-pswp-height="' + imageDimensions.height + '" target="_blank"><img src="' + link + '" alt=""></a>';
                carouselImages.innerHTML = carouselImages.innerHTML + a;
            })
            .catch((error) => {
                console.error(error);
            });
    });
    carouselImages.innerHTML = carouselImages.innerHTML + '</div>';


    createCarousel();


    return document.getElementById("info-carousel");
}
function createCarousel() {
    var lightbox = new PhotoSwipeLightbox({
        gallery: '#info-carousel',
        children: 'a',
        pswpModule: () => import('https://unpkg.com/photoswipe/dist/photoswipe.esm.js'),
        wheelToZoom: true,
        secondaryZoomLevel: 1,
        maxZoomLevel: 40,
        imageClickAction: 'zoom',
    });
    lightbox.on('uiRegister', function () {
        lightbox.pswp.ui.registerElement({
            name: 'rotate-button',
            ariaLabel: 'Toggle zoom',
            order: 10,
            isButton: true,
            html: '',
            onClick: (event, el) => {
                var currentSlide = lightbox.pswp.currSlide;
                const currentSlideContainer = currentSlide.container;
                const currentImage = currentSlideContainer.querySelector('img');

                var currentRotation = parseInt((currentImage.getAttribute('data-rotation') || 0)) + 90;
                currentImage.style.transform = 'rotate(' + currentRotation + 'deg)';
                currentImage.setAttribute('data-rotation', currentRotation);
            }
        });

        lightbox.pswp.ui.registerElement({
            name: 'zoom-level-indicator',
            order: 11,
            onInit: (el, pswp) => {
                pswp.on('zoomPanUpdate', (e) => {
                    if (e.slide === pswp.currSlide) {
                        el.innerText = 'Zoom level is ' + Math.round(pswp.currSlide.currZoomLevel * 100) + '%';
                    }
                });
            }
        });


    });
    lightbox.init();
}

function buscadorPorId() {
    var researchId = JSON.parse(document.getElementById("ResearchId").getAttribute("data-researchId"));
    $("#ResearchId").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: researchId.map(function (id) {
            return { text: id.Text.toString(), value: id.Value };
        }),
        optionLabel: "Todos",
        filter: "contains",
        change: function () {
            busquedaFiltrada();
        }
    });
}