const TaskManager = {
    init: function () {
        this.bindEvents();
        this.initFooterTips();
        this.initModalFocus();
    },

    bindEvents: function () {
        document.addEventListener("click", (e) => this.handleModalClick(e));
        document.addEventListener("change", (e) => {
            if (e.target.classList.contains('priority-select-direct')) {
                this.handlePriorityUpdate(e);
            }
            this.handleTagValidation(e);
        });
        document.addEventListener("submit", (e) => this.handleFormSubmit(e), true);
    },

    initModalFocus: function () {
        const modalEl = document.getElementById('taskModal');
        if (!modalEl) return;
        modalEl.addEventListener('shown.bs.modal', () => {
            const titleInput = document.querySelector('#taskModalBody input[name="Title"], #taskModalBody #Title');
            if (titleInput) titleInput.focus();
        });
    },

    handleModalClick: function (e) {
        const btn = e.target.closest("[data-modal-url]");
        if (!btn) return;

        e.preventDefault();
        e.stopPropagation();

        const url = btn.getAttribute("data-modal-url");
        const title = btn.getAttribute("data-modal-title") || "Task Details";

        fetch(url)
            .then(r => r.text())
            .then(html => {
                const modalBody = document.getElementById("taskModalBody");
                const modalTitle = document.getElementById("taskModalTitle");

                if (modalBody) modalBody.innerHTML = html;
                if (modalTitle) modalTitle.textContent = title;

                const modalEl = document.getElementById("taskModal");
                let modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                modal.show();
                this.rebindValidation();
            })
            .catch(() => alert("Failed to load modal content."));
    },

    handlePriorityUpdate: function (e) {
        const select = e.target;
        const newPriority = select.value; 
        const card = select.closest('.task-card');
        const form = select.closest('form');

        fetch(form.action, {
            method: "POST",
            body: new FormData(form),
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
            .then(response => {
                if (response.ok) {
                    const themes = { "1": "success", "2": "warning", "3": "orange", "4": "danger" };
                    const newTheme = themes[newPriority] || "success";

                    select.className = `form-select form-select-sm priority-select-direct pathly-dropdown bg-soft-${newTheme}`;

                    card.setAttribute('data-priority-theme', newTheme);
                }
            });
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
                    if (window.location.pathname.includes("Roadmap")) {
                        location.reload();
                    } else {
                        window.location.href = '/Tasks/Index';
                    }
                } else if (isHtml) {
                    const html = await response.text();
                    document.getElementById("taskModalBody").innerHTML = html;
                    this.rebindValidation();
                }
            });
    },

    rebindValidation: function () {
        if (window.jQuery && $.validator) {
            const form = document.querySelector("#taskModalBody form");
            if (form) $.validator.unobtrusive.parse(form);
        }
    },

    handleTagValidation: function (e) {
        if (e.target.classList.contains('tag-checker')) {
            const group = e.target.closest('.tag-checkbox-group');
            const checkedCount = group.querySelectorAll('.tag-checker:checked').length;
            const errorSpan = document.getElementById('TagError');
            const submitBtn = document.querySelector('button[type="submit"]');
            const isInvalid = checkedCount > 4;
            if (errorSpan) errorSpan.classList.toggle('d-none', !isInvalid);
            if (submitBtn) submitBtn.disabled = isInvalid;
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

const RoadmapPlanner = {
    toggleTask: function (taskId, actionId) {
        const card = document.getElementById(`planner-card-${taskId}`);
        if (!card) return;

        const isSelected = card.classList.contains('selected');
        const url = isSelected ? '/Roadmap/UnlinkTask' : '/Roadmap/LinkTask';
        const params = isSelected ? `taskId=${taskId}` : `taskId=${taskId}&actionId=${actionId}`;

        fetch(`${url}?${params}`, {
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(response => {
                if (response.ok) {
                    card.classList.toggle('selected');
                } else {
                    alert("Failed to update task linkage.");
                }
            })
            .catch(err => console.error("Planner Error:", err));
    },

    deleteTaskPermanently: function (taskId) {
        if (!confirm("Are you sure? This will delete the task from your database entirely.")) return;

        fetch(`/Tasks/Delete/${taskId}`, {
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(response => {
                if (response.ok) {
                    const item = document.getElementById(`task-item-${taskId}`);
                    if (item) {
                        item.style.opacity = '0';
                        setTimeout(() => item.remove(), 300);
                    }
                    const plannerCard = document.getElementById(`task-wrapper-${taskId}`);
                    if (plannerCard) {
                        plannerCard.style.opacity = '0';
                        setTimeout(() => plannerCard.remove(), 300);
                    }
                } else {
                    alert("Could not delete task.");
                }
            });
    },

    dismissLocally: function (taskId) {
        const wrapper = document.getElementById(`task-wrapper-${taskId}`);
        if (wrapper) {
            wrapper.style.opacity = '0';
            wrapper.style.transform = 'scale(0.8)';
            setTimeout(() => wrapper.remove(), 300);
        }
    }
};

document.addEventListener("DOMContentLoaded", () => TaskManager.init());