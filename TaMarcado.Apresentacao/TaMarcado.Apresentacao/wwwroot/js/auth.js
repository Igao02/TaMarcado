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
