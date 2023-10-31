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


