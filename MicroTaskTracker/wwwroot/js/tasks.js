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

});
