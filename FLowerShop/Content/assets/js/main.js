// Xá»­ lÃ½ sá»± kiá»‡n click ngoÃ i collapse menu Ä‘á»ƒ Ä‘Ã³ng menu
document.addEventListener("click", function(event) {
  var collapseMenu = document.getElementById("navbarMenu");
  var targetElement = event.target;

  var isClickInside = collapseMenu.contains(targetElement);

  if (!isClickInside || targetElement.matches("#loginBtn")) {
    collapseMenu.classList.remove("show");
  }
});

const toastTriggers = document.querySelectorAll('.liveToastBtn');
const toastMessage = document.querySelector('.liveToast');

if (toastTriggers.length > 0) {
  toastTriggers.forEach((trigger) => {
    trigger.addEventListener('click', () => {
      const toastBootstrap = new bootstrap.Toast(toastMessage);
      toastBootstrap.show();
    });
  });
}

window.onpopstate = function (event) {
    location.reload();
};


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