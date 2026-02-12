const TaskManager = {
    init: function () {
        this.bindEvents();
        this.initFooterTips();
    },

    bindEvents: function () {
        // Modal Loading
        document.addEventListener("click", (e) => this.handleModalClick(e));

        // Tag Validation (Checkboxes)
        document.addEventListener("change", (e) => this.handleTagValidation(e));

        // Form Submission
        document.addEventListener("submit", (e) => this.handleFormSubmit(e), true);
    },

    handleModalClick: function (e) {
        const btn = e.target.closest("[data-modal-url]");
        if (!btn) return;
        e.preventDefault();

        fetch(btn.getAttribute("data-modal-url"))
            .then(r => r.text())
            .then(html => {
                document.getElementById("taskModalBody").innerHTML = html;
                document.getElementById("taskModalTitle").textContent = btn.getAttribute("data-modal-title") || "Task";

                const modalEl = document.getElementById("taskModal");
                let modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                modal.show();

                this.rebindValidation();
            })
            .catch(() => alert(AppConfig.Errors.LoadFailed));
    },

    handleTagValidation: function (e) {
        if (e.target.classList.contains('tag-checker')) {
            const group = e.target.closest('.tag-checkbox-group');
            const checkedCount = group.querySelectorAll('.tag-checker:checked').length;
            const errorSpan = document.getElementById('TagError');
            const submitBtn = document.querySelector('button[type="submit"]');

            const isInvalid = checkedCount > 4;
            errorSpan.classList.toggle('d-none', !isInvalid);
            if (submitBtn) submitBtn.disabled = isInvalid;
        }
    },

    handleFormSubmit: function (e) {
        const form = e.target.closest(".task-form");
        if (!form) return;

        e.preventDefault();
        const formData = new FormData(form);

        fetch(form.getAttribute("action"), {
            method: "POST",
            body: formData,
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
            .then(async response => {
                const isHtml = response.headers.get("content-type")?.includes("text/html");

                if (response.ok && !isHtml) {
                    window.location.href = '/Tasks/Index';
                } else if (isHtml) {
                    const html = await response.text();
                    const bodyEl = document.getElementById("taskModalBody");
                    bodyEl.innerHTML = html;
                    this.rebindValidation();
                }
            })
            .catch(() => alert(AppConfig.Errors.SaveFailed));
    },

    rebindValidation: function () {
        if (window.jQuery && $.validator) {
            const form = document.querySelector("#taskModalBody form");
            if (form) {
                $.validator.unobtrusive.parse(form);
            }
        }
    },

    initFooterTips: function () {
        const tipText = document.getElementById("productivity-tip");
        const rerollBtn = document.getElementById("reroll-tip");

        if (!rerollBtn || !tipText) return;

        rerollBtn.addEventListener("click", () => {
            let newTip;
            do {
                newTip = AppConfig.ProductivityTips[Math.floor(Math.random() * AppConfig.ProductivityTips.length)];
            } while (newTip === tipText.innerText);

            tipText.style.opacity = 0;
            setTimeout(() => {
                tipText.innerText = newTip;
                tipText.style.opacity = 1;
            }, 200);
        });
    }
};

// Initialize on Load
document.addEventListener("DOMContentLoaded", () => TaskManager.init());