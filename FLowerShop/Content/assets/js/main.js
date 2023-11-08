document.addEventListener("click", function(event) {
  var collapseMenu = document.getElementById("navbarMenu");
  var targetElement = event.target;

  var isClickInside = collapseMenu.contains(targetElement);

  if (!isClickInside || targetElement.matches("#loginBtn")) {
    collapseMenu.classList.remove("show");
  }
});

document.addEventListener("DOMContentLoaded", function () {
    var navLinks = document.querySelectorAll("a.link-select");
    navLinks.forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault();

            var currentUrl = window.location.pathname;
            var targetUrl = this.getAttribute("data-url");
            if (currentUrl !== targetUrl) {
                window.location.href = targetUrl;
            }
        });
    });
});
