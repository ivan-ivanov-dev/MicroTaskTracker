(function () {
    'use strict';

    // ============================================
    // HIGHLIGHT ACTIVE NAV LINK
    // ============================================
    function highlightActiveNavLink() {
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.navbar-nav .nav-link');

        navLinks.forEach(link => {
            link.classList.remove('active');
            if (link.getAttribute('href') === currentPath) {
                link.classList.add('active');
            }
        });
    }

    // ============================================
    // AUTO-SUBMIT ON SELECT CHANGE
    // ============================================
    function initAutoSubmitSelects() {
        const autoSubmitSelects = document.querySelectorAll('select[data-auto-submit="true"]');

        autoSubmitSelects.forEach(select => {
            select.addEventListener('change', function () {
                const form = this.closest('form');
                if (form) {
                    form.submit();
                }
            });
        });
    }

    // ============================================
    // CONFIRM DELETE DIALOGS
    // ============================================
    function initDeleteConfirmations() {
        const deleteLinks = document.querySelectorAll('[data-confirm-delete="true"]');

        deleteLinks.forEach(link => {
            link.addEventListener('click', function (e) {
                const confirmed = confirm('Are you sure you want to delete this item? This action cannot be undone.');
                if (!confirmed) {
                    e.preventDefault();
                    return false;
                }
            });
        });
    }

    // ============================================
    // SMOOTH FADE-IN FOR CONTENT
    // ============================================
    function initFadeInAnimation() {
        const mainContent = document.querySelector('.main-content');
        if (mainContent) {
            mainContent.style.opacity = '0';
            setTimeout(() => {
                mainContent.style.transition = 'opacity 0.3s ease-in';
                mainContent.style.opacity = '1';
            }, 10);
        }
    }

    // ============================================
    // INITIALIZE ON DOM READY
    // ============================================
    document.addEventListener('DOMContentLoaded', function () {
        highlightActiveNavLink();
        initAutoSubmitSelects();
        initDeleteConfirmations();
        initFadeInAnimation();
    });
})();