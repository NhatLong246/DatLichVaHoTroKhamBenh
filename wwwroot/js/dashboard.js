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

// Global Toast System
window.showToast = function (message, type = "error") {
  let container = document.querySelector(".app-toast-container");
  if (!container) {
    container = document.createElement("div");
    container.className = "app-toast-container";
    document.body.appendChild(container);
  }

  const toast = document.createElement("div");
  toast.className = `app-toast toast-${type}`;
  
  let icon = "";
  if (type === "error") {
      icon = `<svg viewBox="0 0 24 24" fill="none" stroke="#ef4444" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>`;
  } else if (type === "warning") {
      icon = `<svg viewBox="0 0 24 24" fill="none" stroke="#f59e0b" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>`;
  } else {
      icon = `<svg viewBox="0 0 24 24" fill="none" stroke="#10b981" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>`;
  }

  toast.innerHTML = `${icon} <span>${message}</span>`;
  container.appendChild(toast);

  // Trigger reflow to start animation
  void toast.offsetWidth;
  toast.classList.add("is-visible");

  setTimeout(() => {
    toast.classList.remove("is-visible");
    setTimeout(() => {
        toast.remove();
        if (container.children.length === 0) {
            container.remove();
        }
    }, 400);
  }, 3500);
};
