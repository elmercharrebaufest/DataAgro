import PhotoSwipeLightbox from 'https://unpkg.com/photoswipe/dist/photoswipe-lightbox.esm.js';
var markers;
var map = L.map('map').setView([-38.678907, -61.104765], 5);
var selectedGrano = "Todos";
var selectedCampania = "Todos";
var addressPoints = [{}];

// Obtén una referencia al botón y al panel de filtros
const filterButton = document.getElementById("filter-button");
const filterForm = document.getElementById("filter-form");

$(document).ready(function () {
    busquedaFiltrada();
    updateMarkers(selectedGrano, selectedCampania);
});

// Agrega un evento clic al botón para mostrar/ocultar el panel de filtros
filterButton.addEventListener("click", () => {
    if (filterForm.style.display === "none") {
        filterForm.style.display = "block";
    } else {
        filterForm.style.display = "none";
    }
});

// Agrega un evento de cambio al selector de granos
document.getElementById("grano-filter").addEventListener("change", function () {
    selectedGrano = this.value;
    console.log(`${selectedGrano} - ${selectedCampania}`);
    busquedaFiltrada();
    updateMarkers(selectedGrano, selectedCampania);
});

document.getElementById("campania-filter").addEventListener("change", function () {
    selectedCampania = this.value;
    console.log(`${selectedGrano} - ${selectedCampania}`);
    busquedaFiltrada();
    updateMarkers(selectedGrano, selectedCampania);
});

function busquedaFiltrada() {
    // /ResearchReporte/BuscaDatosTabla

    //var param = {
    //    "Nombre": viewModel.get("Parametros.Nombre"),
    //    "ProvinciaId": GetDropDownValue(viewModel, "Parametros.ProvinciaId.ProvinciaId"),
    //    "PartidoId": partidoId
    //};

    var result = MSExecuteOnServer('/ResearchMapa/BuscaDatosTabla');

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
        tipoMuestra1: x.TipoMuestraUno,
        medida1: x.MedidasUno,
        promedio1: x.PromedioMuestraUno,
        tipoMuestra2: x.TipoMuestraDos,
        medida2: x.MedidasDos,
        promedio2: x.PromedioMuestraDos,
        tipoMuestra3: x.TipoMuestraTres,
        medida3: x.MedidasTres,
        promedio3: x.PromedioMuestraTres,
        lat: x.Latitud,
        lng: x.Longitud,
        id: x.Id,
        imgs: [
            "https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg",
            "https://fastly.picsum.photos/id/866/2000/3000.jpg?hmac=_X5OfUFRDyrhMZM4Z43W_pCrWO3_3tFYaAa_MWe23f0"
        ]
    }))
}

function updateMarkers(selectedGrano, selectedCampania) {

    // Filtra los marcadores según el grano seleccionado
    var filteredMarkers = addressPoints.filter(function (marker) {
        return ((marker.grano === selectedGrano || selectedGrano == "Todos") && (marker.campania === selectedCampania || selectedCampania == "Todos"));
    });

    map.removeLayer(markers);

    markers = L.markerClusterGroup({
        maxClusterRadius: 80
    });

    for (var i = 0; i < filteredMarkers.length; i++) {
        var a = filteredMarkers[i];
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
}


//addressPoints = [
//    { tipoCarga: "Express", grano: "Soja", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -31.8325816, lng: -64.2238798667, id: "537", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://fastly.picsum.photos/id/866/2000/3000.jpg?hmac=_X5OfUFRDyrhMZM4Z43W_pCrWO3_3tFYaAa_MWe23f0"] },
//    { tipoCarga: "Express", grano: "Soja", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -31.8210922667, lng: -64.2209316333, id: "2", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Maiz", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -31.8210819833, lng: -64.2213903167, id: "3", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Trigo", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -31.8210881833, lng: -64.2215004833, id: "3A", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Girasol", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -36.8211946833, lng: -60.2213655333, id: "1", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Trigo", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -36.8209458667, lng: -60.2214051333, id: "5", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Girasol", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -36.8208292333, lng: -60.2214374833, id: "7", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Soja", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -36.8315855167, lng: -60.2279767, id: "454", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Girasol", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8096336833, lng: -61.2223743833, id: "176", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Maiz", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.80970685, lng: -61.2221815833, id: "178", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Soja", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8102146667, lng: -61.2211562833, id: "190", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Girasol", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8088037167, lng: -61.2242227, id: "156", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Soja", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8112330167, lng: -61.2193425667, id: "210", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Trigo", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8116368667, lng: -61.2193005167, id: "212", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Maiz", antecesor: "Trigo", campania: "21-22", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.80812645, lng: -61.2255449333, id: "146", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Trigo", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8080231333, lng: -61.2286383167, id: "125", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Soja", antecesor: "Trigo", campania: "22-23", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8089538667, lng: -61.2222222333, id: "174", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] },
//    { tipoCarga: "Express", grano: "Maiz", antecesor: "Trigo", campania: "21-22", estadoFenologico: "xxxxx", condicion: "Buena", humedad: "poca", comentarios: "asdas", provincia: "Bs As", partido: "Caba", localidad: "Palermo", rendimiento: 34, tipoMuestra1: "Granos por hilera", medida1: "33, 44, 55", promedio1: 44, tipoMuestra2: "Granos por hilera", medida2: "33, 44, 55", promedio2: 44, tipoMuestra3: "Granos por hilera", medida3: "33, 44, 55", promedio3: 44, lat: -32.8080905833, lng: -61.2275400667, id: "129", imgs: ["https://media.traveler.es/photos/63f0b0c5834b3e6c89f3d30f/16:9/w_2560%2climit/BAP0A6%2520(1).jpg", "https://www.buscounchollo.com/blog/app/uploads/2022/08/AdobeStock_280800797.jpg"] }
//];

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

// Definir una función para cargar la información del popup
function cargarInformacionPopup(popup) {
    if (document.getElementById("info-carousel") == null) {
        console.log("null document.getElementById info - carousel");
        var elemDiv = document.createElement('div');
        elemDiv.setAttribute("id", "info-carousel");
        document.body.appendChild(elemDiv);
    }

    //document.getElementById("info-carousel").innerHTML = ' <div class="info-carousel" id="info-carousel" class="leaflet-popup-content"> <div id="carousel-images"> </div><div class="row"> <div class="col-md-12"><span class="bold-text">#</span> <span id="id"></span></div><div class="col-md-12"><span class="bold-text">Tipo de carga:</span> <span id="tipoCarga"></span></div><div class="col-md-12"><span class="bold-text">Grano:</span> <span id="grano"></span></div><div class="col-md-12"><span class="bold-text">Antecesor:</span> <span id="antecesor"></span></div><div class="col-md-12"><span class="bold-text">Campania:</span> <span id="campania"></span></div><div class="col-md-12"><span class="bold-text">Estado Fenológico:</span> <span id="estadoFenologico"></span></div><div class="col-md-12"><span class="bold-text">Condición del Cultivo:</span> <span id="condicion"></span></div><div class="col-md-12"><span class="bold-text">Humedad del Suelo:</span> <span id="humedad"></span></div><div class="col-md-12"><span class="bold-text">Comentarios:</span> <span id="comentarios"></span></div><div class="col-md-12"><span class="bold-text">Hubicacion:</span> <span id="hubicacion"></span></div><div class="col-md-12"><span class="bold-text">Rendimiento:</span> <span id="rendimiento"></span></div><div class="col-md-12"><span class="bold-text" id="tipoMuestra1"></span> <span id="medida1"></span></div><div class="col-md-12"><span class="bold-text" id="tipoMuestra2"></span> <span id="medida2"></span></div><div class="col-md-12"><span class="bold-text" id="tipoMuestra3"></span> <span id="medida3"></span></div></div></div>';
    document.getElementById("info-carousel").innerHTML = ''
        + '<div class="info-carousel" id="info-carousel" class="leaflet-popup-content">'
        + '<div id="carousel-images"> </div>'
        + '<div class="row">'
        + '<div class="col-md-12"><span class="bold-text">#</span> <span id="id"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Tipo de carga:</span> <span id="tipoCarga"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Grano:</span> <span id="grano"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Antecesor:</span> <span id="antecesor"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Campaña:</span> <span id="campania"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Estado Fenológico:</span> <span id="estadoFenologico"></span>'
        + '</div>'
        + '<div class="col-md-12"><span class="bold-text">Condición del Cultivo:</span> <span id="condicion"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Humedad del Suelo:</span> <span id="humedad"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Comentarios:</span> <span id="comentarios"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Hubicacion:</span> <span id="hubicacion"></span></div>'
        + '<div class="col-md-12"><span class="bold-text">Rendimiento:</span> <span id="rendimiento"></span></div>'
        + '<div class="col-md-12"><span class="bold-text" id="tipoMuestra1"></span> <span id="medida1"></span></div>'
        + '<div class="col-md-12"><span class="bold-text" id="tipoMuestra2"></span> <span id="medida2"></span></div>'
        + '<div class="col-md-12"><span class="bold-text" id="tipoMuestra3"></span> <span id="medida3"></span></div>'
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
    document.getElementById("hubicacion").innerHTML = `${popup.options.provincia == null ? "" : (popup.options.provincia + ", ")} ${popup.options.partido == null ? "" : (popup.options.partido + ", ")} ${popup.options.localidad == null ? "" : popup.options.localidad}`;
    document.getElementById("rendimiento").innerHTML = popup.options.rendimiento;

    if (popup.options.tipoMuestra1 != null && popup.options.tipoMuestra1 != "") {
        document.getElementById("tipoMuestra1").innerHTML = popup.options.tipoMuestra1;
        document.getElementById("medida1").innerHTML = `${popup.options.promedio1} (${popup.options.medida1})`;
    }
    if (popup.options.tipoMuestra2 != null && popup.options.tipoMuestra2 != "") {
        document.getElementById("tipoMuestra2").innerHTML = popup.options.tipoMuestra2;
        document.getElementById("medida2").innerHTML = `${popup.options.promedio2} (${popup.options.medida2})`;
    }
    if (popup.options.tipoMuestra3 != null && popup.options.tipoMuestra3 != "") {
        document.getElementById("tipoMuestra3").innerHTML = popup.options.tipoMuestra3;
        document.getElementById("medida3").innerHTML = `${popup.options.promedio3} (${popup.options.medida3})`;
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
                console.error(error); // Manejar errores, si los hay
            });
    });
    carouselImages.innerHTML = carouselImages.innerHTML + '</div>';


    createCarousel();


    return document.getElementById("info-carousel");
}

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


markers = L.markerClusterGroup({
    maxClusterRadius: 80
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
        console.log(2)
        console.log(lightbox.pswp)
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