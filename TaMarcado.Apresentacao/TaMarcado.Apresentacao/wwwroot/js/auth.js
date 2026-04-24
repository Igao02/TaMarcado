window.loginWithFetch = async function (email, password) {
    try {
        const response = await fetch('/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password }),
            credentials: 'include'
        });
        return response.ok;
    } catch {
        return false;
    }
};

document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form[action="/auth/login"]');
    if (!form) return;
    form.addEventListener('submit', function () {
        const btn = form.querySelector('button[type="submit"]');
        if (btn) btn.disabled = true;
    });
});

window.logoutWithFetch = async function () {
    try {
        const response = await fetch('/auth/logout', {
            method: 'POST',
            credentials: 'include'
        });
        return response.ok;
    } catch {
        return false;
    }
};
