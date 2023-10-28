// Xử lý sự kiện click ngoài collapse menu để đóng menu
document.addEventListener("click", function(event) {
  var collapseMenu = document.getElementById("navbarMenu");
  var targetElement = event.target;

  var isClickInside = collapseMenu.contains(targetElement);

  if (!isClickInside || targetElement.matches("#loginBtn")) {
    collapseMenu.classList.remove("show");
  }
});

const toastTriggers = document.querySelectorAll('.liveToastBtn');
const toastLiveExample = document.querySelector('.liveToast');

if (toastTriggers.length > 0) {
  toastTriggers.forEach((trigger) => {
    trigger.addEventListener('click', () => {
      const toastBootstrap = new bootstrap.Toast(toastLiveExample);
      toastBootstrap.show();
    });
  });
}


