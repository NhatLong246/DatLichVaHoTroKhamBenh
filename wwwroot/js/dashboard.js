(function () {
  const confirmForms = document.querySelectorAll(".confirm-form, .logout-form");

  if (confirmForms.length === 0) {
    return;
  }

  let activeForm = null;

  const modal = document.createElement("div");
  modal.className = "app-confirm";
  modal.innerHTML = `
    <div class="app-confirm__backdrop" data-confirm-cancel></div>
    <section class="app-confirm__dialog" role="dialog" aria-modal="true" aria-labelledby="appConfirmTitle">
      <div class="app-confirm__icon">?</div>
      <div class="app-confirm__content">
        <h2 id="appConfirmTitle">Xác nhận thao tác</h2>
        <p>Bạn có chắc chắn muốn tiếp tục?</p>
      </div>
      <div class="app-confirm__actions">
        <button type="button" class="app-confirm__button app-confirm__button--ghost" data-confirm-cancel>Ở lại</button>
        <button type="button" class="app-confirm__button app-confirm__button--primary" data-confirm-ok>Tiếp tục</button>
      </div>
    </section>
  `;
  document.body.appendChild(modal);

  const titleElement = modal.querySelector(".app-confirm__content h2");
  const messageElement = modal.querySelector(".app-confirm__content p");
  const cancelButtons = modal.querySelectorAll("[data-confirm-cancel]");
  const okButton = modal.querySelector("[data-confirm-ok]");

  function openModal(form) {
    activeForm = form;
    const isLogout = form.classList.contains("logout-form");
    titleElement.textContent = form.dataset.confirmTitle || (isLogout ? "Xác nhận đăng xuất" : "Xác nhận thao tác");
    messageElement.textContent = form.dataset.confirmMessage || (isLogout ? "Bạn có chắc chắn muốn đăng xuất không?" : "Bạn có chắc chắn muốn tiếp tục?");
    okButton.textContent = form.dataset.confirmOk || (isLogout ? "Đăng xuất" : "Tiếp tục");
    
    // Update cancel button text if provided
    const cancelButton = modal.querySelector(".app-confirm__button--ghost");
    if (cancelButton) {
        cancelButton.textContent = form.dataset.confirmCancel || "Ở lại";
    }

    modal.classList.add("is-open");
    okButton.focus();
  }

  function closeModal() {
    modal.classList.remove("is-open");
    activeForm = null;
  }

  confirmForms.forEach((form) => {
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
