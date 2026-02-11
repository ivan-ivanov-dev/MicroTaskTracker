document.addEventListener('DOMContentLoaded', function () {
    // Fade-in animation for auth card
    const authForm = document.querySelector('.auth-form');
    if (authForm) {
        setTimeout(() => authForm.classList.add('fade-in-up'), 100);
    }

    // Password strength for register form
    var pwdInput = document.querySelector('#Password');
    var pwdStrengthEl = document.querySelector('#passwordStrength');
    if (pwdInput && pwdStrengthEl) {
        pwdInput.addEventListener('input', function () {
            var val = pwdInput.value || '';
            var score = 0;
            if (val.length >= 8) score++;
            if (/[A-Z]/.test(val)) score++;
            if (/[0-9]/.test(val)) score++;
            if (/[^A-Za-z0-9]/.test(val)) score++;
            pwdStrengthEl.classList.remove('weak', 'fair', 'good', 'strong');
            if (score <= 1) pwdStrengthEl.classList.add('weak');
            else if (score === 2) pwdStrengthEl.classList.add('fair');
            else if (score === 3) pwdStrengthEl.classList.add('good');
            else pwdStrengthEl.classList.add('strong');
        });
    }

    // Disable submit if required fields empty
    function attachDisableSubmit(formSelector, requiredSelectors) {
        var form = document.querySelector(formSelector);
        if (!form) return;
        var submit = form.querySelector('button[type="submit"]');
        if (!submit) return;
        function update() {
            var ok = requiredSelectors.every(function (sel) {
                var el = form.querySelector(sel);
                return el && el.value.trim().length > 0;
            });
            submit.disabled = !ok;
        }
        requiredSelectors.forEach(function (sel) {
            var el = form.querySelector(sel);
            if (el) el.addEventListener('input', update);
        });
        update();
    }

    attachDisableSubmit('#registerForm', ['#UserName', '#Email', '#Password', '#ConfirmPassword']);
    attachDisableSubmit('#loginForm', ['#LoginIdentifier', '#Password']);
});
