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
