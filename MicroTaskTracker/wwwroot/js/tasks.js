document.addEventListener("DOMContentLoaded", function () {

    // Auto-submit forms when priority changes
    document.querySelectorAll("select[data-auto-submit='true']").forEach(select => {
        select.addEventListener("change", function () {
            this.form.submit();
        });
    });

    // Confirm delete
    document.querySelectorAll("a[data-confirm-delete='true']").forEach(link => {
        link.addEventListener("click", function (e) {
            const ok = confirm("Are you sure you want to delete this task?");
            if (!ok) {
                e.preventDefault();
            }
        });
    });

    // Delegate click to load partials into modal (supports existing modal ids)
    document.addEventListener("click", function (e) {
        const btn = e.target.closest("[data-modal-url]");
        if (!btn) return;

        e.preventDefault();

        const url = btn.getAttribute("data-modal-url");
        if (!url) return;

        fetch(url, { credentials: 'same-origin' })
            .then(r => {
                if (!r.ok) throw new Error(`Failed to load: ${r.statusText}`);
                return r.text();
            })
            .then(html => {
                // Prefer modal with id "taskModal", fall back to "createTaskModal"
                const bodyEl = document.getElementById("taskModalBody")
                    || document.querySelector("#createTaskModal .modal-body");
                const titleEl = document.getElementById("taskModalTitle")
                    || document.querySelector("#createTaskModal .modal-title");
                const modalEl = document.getElementById("taskModal") || document.getElementById("createTaskModal");

                if (bodyEl) {
                    bodyEl.innerHTML = html;
                } else {
                    console.warn("Modal body element not found to inject HTML.");
                }

                if (titleEl) {
                    const title = btn.getAttribute("data-modal-title") || titleEl.textContent;
                    titleEl.textContent = title;
                }

                if (modalEl) {
                    const modal = new bootstrap.Modal(modalEl);
                    modal.show();
                } else {
                    console.warn("Modal element not found to show.");
                }
            })
            .catch(err => {
                console.error(err);
                if (window.showAlert) {
                    window.showAlert("Failed to load dialog. Please try again.", "danger", true, false);
                } else {
                    alert("Failed to load dialog. Please try again.");
                }
            });
    });
    document.addEventListener("submit", function (e) {
        const form = e.target;
        if (!form.matches("#createTaskForm")) return;

        e.preventDefault();

        const formData = new FormData(form);

        fetch(form.action, {
            method: "POST",
            body: formData,
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        })
            .then(r => {
                if (r.status === 200 && r.headers.get("content-type")?.includes("text/html")) {
                    return r.text(); // validation errors → return partial again
                }
                if (r.ok) return null; // success
                throw new Error("Request failed");
            })
            .then(html => {
                if (html) {
                    // Re-render partial with validation errors
                    document.getElementById("taskModalBody").innerHTML = html;
                } else {
                    // Success → close modal and reload page
                    const modalEl = document.getElementById("taskModal");
                    const modal = bootstrap.Modal.getInstance(modalEl);
                    modal.hide();
                    window.location.reload();
                }
            })
            .catch(err => {
                console.error(err);
                alert("Failed to save task.");
            });
    });


});
