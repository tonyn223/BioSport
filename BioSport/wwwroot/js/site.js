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
