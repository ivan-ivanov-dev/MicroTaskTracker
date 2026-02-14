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
                if (form) form.submit();
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
                if (!confirmed) e.preventDefault();
            });
        });
    }

    // ============================================
    // FADE-IN ON SCROLL (sections + cards)
    // ============================================
    function initFadeInOnScroll() {
        const fadeElements = document.querySelectorAll('section, .card');

        const observer = new IntersectionObserver((entries, obs) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('fade-in-up');
                    obs.unobserve(entry.target);
                }
            });
        }, { threshold: 0.15 });

        fadeElements.forEach(el => observer.observe(el));
    }

    // ============================================
    // HERO SECTION STAGGERED ANIMATION
    // ============================================
    function initHeroAnimation() {
        const heroItems = document.querySelectorAll('.py-5 .col-lg-6 > *');
        if (!heroItems.length) return;

        heroItems.forEach((item, index) => {
            item.style.opacity = 0;
            item.style.transform = 'translateY(20px)';
            setTimeout(() => {
                item.style.transition = `opacity 0.6s ease-out ${index * 0.2}s, transform 0.6s ease-out ${index * 0.2}s`;
                item.style.opacity = 1;
                item.style.transform = 'translateY(0)';
            }, 100);
        });
    }

    // ============================================
    // CARDS & BUTTONS HOVER EFFECTS
    // ============================================
    function initHoverEffects() {
        const hoverCards = document.querySelectorAll('.card');
        hoverCards.forEach(card => {
            card.addEventListener('mouseenter', () => {
                card.style.transition = 'transform 0.3s ease, box-shadow 0.3s ease';
                card.style.transform = 'translateY(-5px)';
                card.style.boxShadow = 'var(--shadow-lg)';
            });
            card.addEventListener('mouseleave', () => {
                card.style.transform = 'translateY(0)';
                card.style.boxShadow = 'var(--shadow-sm)';
            });
        });

        const hoverButtons = document.querySelectorAll('.btn');
        hoverButtons.forEach(btn => {
            btn.addEventListener('mouseenter', () => {
                btn.style.transition = 'transform 0.2s ease, box-shadow 0.2s ease';
                btn.style.transform = 'translateY(-2px)';
                btn.style.boxShadow = 'var(--shadow-md)';
            });
            btn.addEventListener('mouseleave', () => {
                btn.style.transform = 'translateY(0)';
                btn.style.boxShadow = 'none';
            });
        });
    }

    // ============================================
    // INITIALIZE ALL
    // ============================================
    document.addEventListener('DOMContentLoaded', function () {
        highlightActiveNavLink();
        initAutoSubmitSelects();
        initDeleteConfirmations();
        initFadeInOnScroll();
        initHeroAnimation();
        initHoverEffects();
    });
})();
