(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            var logo = document.getElementsByClassName("link");
            logo[0].href = "https://github.com/matjazbravc";
            logo[0].target = "_blank";
            logo[0].children[0].alt = "Implemeting Swagger";
            logo[0].children[0].src = "/swagger/company-logo-swagger.png";
        });
    });
})();