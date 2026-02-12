// 1. MODAL LOADING LOGIC
document.addEventListener("click", function (e) {
    const btn = e.target.closest("[data-modal-url]");
    if (!btn) return;

    e.preventDefault();
    const url = btn.getAttribute("data-modal-url");

    fetch(url, { credentials: "same-origin" })
        .then(r => {
            if (!r.ok) throw new Error("Failed to load");
            return r.text();
        })
        .then(html => {
            const bodyEl = document.getElementById("taskModalBody");
            const titleEl = document.getElementById("taskModalTitle");
            const modalEl = document.getElementById("taskModal");

            bodyEl.innerHTML = html;
            titleEl.textContent = btn.getAttribute("data-modal-title") || "Task";

            // Initialize and show the Bootstrap modal
            let modal = bootstrap.Modal.getInstance(modalEl);
            if (!modal) modal = new bootstrap.Modal(modalEl);
            modal.show();

            // Re-bind validation if using jQuery Unobtrusive
            if (window.jQuery && $.validator && $.validator.unobtrusive) {
                const form = bodyEl.querySelector("form");
                if (form) $.validator.unobtrusive.parse(form);
            }
        })
        .catch(err => {
            console.error(err);
            alert("Failed to load dialog.");
        });
});

// 2. FORM SUBMISSION LOGIC
document.addEventListener("submit", function (e) {
    const form = e.target.closest(".task-form");
    if (!form) return;

    // STOP the browser from doing a traditional page reload
    e.preventDefault();
    e.stopPropagation();

    const formData = new FormData(form);
    const actionUrl = form.getAttribute("action");

    fetch(actionUrl, {
        method: "POST",
        body: formData,
        headers: { "X-Requested-With": "XMLHttpRequest" }
    })
        .then(async response => {
            const contentType = response.headers.get("content-type");

            // SUCCESS: Server returned 200 OK (from our return Ok() in Controller)
            if (response.ok && (!contentType || !contentType.includes("text/html"))) {
                const modalEl = document.getElementById('taskModal');
                const modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();

                // Redirect back to Index
                window.location.href = '/Tasks/Index';
                return;
            }

            // FAILURE: Server returned PartialView HTML (validation errors)
            if (contentType && contentType.includes("text/html")) {
                const html = await response.text();
                const bodyEl = document.getElementById("taskModalBody");
                bodyEl.innerHTML = html;

                // CRITICAL: Re-bind validation to the new HTML so errors show up
                if (window.jQuery && $.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(bodyEl.querySelector("form"));
                }
            } else {
                throw new Error("Unexpected response from server");
            }
        })
        .catch(err => {
            console.error("Save error:", err);
            alert("An error occurred. Check the console for details.");
        });
}, true); // 'true' helps catch the event before other handlers