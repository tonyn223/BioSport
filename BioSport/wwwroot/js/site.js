// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    var boton = document.getElementById('theme-toggle');
    if (!boton) return;

    boton.addEventListener('click', function () {
        var actual = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'dark' : 'light';
        var nuevo = actual === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-bs-theme', nuevo);
        localStorage.setItem('biosport-theme', nuevo);
    });
})();

// Toasts flotantes (mensajes de éxito/error tras una acción)
(function () {
    if (typeof bootstrap === 'undefined') return;
    document.querySelectorAll('.toast-biosport').forEach(function (el) {
        new bootstrap.Toast(el, { delay: 4500 }).show();
    });
})();

// Estado de carga en formularios: deshabilita el botón de envío y muestra un spinner
// para que la confirmación se sienta inmediata, incluso con recarga de página completa.
(function () {
    document.querySelectorAll('form').forEach(function (form) {
        form.addEventListener('submit', function () {
            if (form.dataset.sinCarga !== undefined) return;
            if (!form.checkValidity || form.checkValidity()) {
                var boton = form.querySelector('button[type="submit"], input[type="submit"]');
                if (boton && !boton.disabled) {
                    boton.dataset.textoOriginal = boton.innerHTML;
                    boton.disabled = true;
                    boton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Procesando...';
                }
            }
        });
    });
})();

// Carrusel de instalaciones: flechas para desplazar el track horizontal
(function () {
    document.querySelectorAll('.galeria-carrusel').forEach(function (carrusel) {
        var track = carrusel.querySelector('.galeria-track');
        var prev = carrusel.querySelector('.galeria-flecha-prev');
        var next = carrusel.querySelector('.galeria-flecha-next');
        if (!track) return;

        function desplazar(direccion) {
            var slide = track.querySelector('.galeria-slide');
            var distancia = slide ? slide.getBoundingClientRect().width + 20 : track.clientWidth * 0.8;
            track.scrollBy({ left: direccion * distancia, behavior: 'smooth' });
        }

        if (prev) prev.addEventListener('click', function () { desplazar(-1); });
        if (next) next.addEventListener('click', function () { desplazar(1); });
    });
})();

// Lightbox: amplía las fotos de la galería al hacer clic
(function () {
    var overlay = document.getElementById('lightbox-overlay');
    var img = document.getElementById('lightbox-img');
    var cerrar = document.getElementById('lightbox-cerrar');
    if (!overlay || !img) return;

    function abrir(src, alt) {
        img.src = src;
        img.alt = alt || '';
        overlay.classList.add('activo');
        document.body.style.overflow = 'hidden';
    }

    function cerrarLightbox() {
        overlay.classList.remove('activo');
        document.body.style.overflow = '';
        img.src = '';
    }

    document.querySelectorAll('[data-lightbox-src]').forEach(function (el) {
        el.addEventListener('click', function () {
            abrir(el.getAttribute('data-lightbox-src'), el.getAttribute('data-lightbox-alt'));
        });
    });

    cerrar.addEventListener('click', cerrarLightbox);
    overlay.addEventListener('click', function (e) {
        if (e.target === overlay) cerrarLightbox();
    });
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') cerrarLightbox();
    });
})();
