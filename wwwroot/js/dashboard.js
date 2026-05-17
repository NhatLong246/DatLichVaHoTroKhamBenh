(function () {
  const logoutForms = document.querySelectorAll(".logout-form");

  if (logoutForms.length === 0) {
    return;
  }

  let activeForm = null;

  const modal = document.createElement("div");
  modal.className = "app-confirm";
  modal.innerHTML = `
    <div class="app-confirm__backdrop" data-confirm-cancel></div>
    <section class="app-confirm__dialog" role="dialog" aria-modal="true" aria-labelledby="logoutConfirmTitle">
      <div class="app-confirm__icon">?</div>
      <div class="app-confirm__content">
        <h2 id="logoutConfirmTitle">Xác nhận đăng xuất</h2>
        <p>Bạn có chắc chắn muốn đăng xuất không?</p>
      </div>
      <div class="app-confirm__actions">
        <button type="button" class="app-confirm__button app-confirm__button--ghost" data-confirm-cancel>Ở lại</button>
        <button type="button" class="app-confirm__button app-confirm__button--primary" data-confirm-ok>Đăng xuất</button>
      </div>
    </section>
  `;
  document.body.appendChild(modal);

  const messageElement = modal.querySelector(".app-confirm__content p");
  const cancelButtons = modal.querySelectorAll("[data-confirm-cancel]");
  const okButton = modal.querySelector("[data-confirm-ok]");

  function openModal(form) {
    activeForm = form;
    messageElement.textContent = form.dataset.confirmMessage || "Bạn có chắc chắn muốn tiếp tục?";
    modal.classList.add("is-open");
    okButton.focus();
  }

  function closeModal() {
    modal.classList.remove("is-open");
    activeForm = null;
  }

  logoutForms.forEach((form) => {
    form.addEventListener("submit", function (event) {
      event.preventDefault();
      openModal(form);
    });
  });

  cancelButtons.forEach((button) => {
    button.addEventListener("click", closeModal);
  });

  okButton.addEventListener("click", function () {
    if (activeForm) {
      activeForm.submit();
    }
  });

  document.addEventListener("keydown", function (event) {
    if (event.key === "Escape" && modal.classList.contains("is-open")) {
      closeModal();
    }
  });
})();
