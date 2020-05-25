export default {
    saveAuth: (id, token, role, login, password) => {
        let base64Url = token.split('.')[1];
        let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        let jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        let decrypted = JSON.parse(jsonPayload);
        sessionStorage.setItem('tokenKey', JSON.stringify({ userId: id, access_token: token, role: role, login: login, password: password, exp: decrypted.exp }));
    },

    clearAuth: () => {
        sessionStorage.removeItem('tokenKey');
    },

    getId: () => {
        let item = sessionStorage.getItem('tokenKey');
        let id = '';
        if (item) {
            id = JSON.parse(item).userId;
        }
        return id;
    },

    isLogged: () => {
        let item = sessionStorage.getItem('tokenKey');
        if (item) {
            return true;
        } else {
            return false;
        }
    },

    getRole: () => {
        let item = sessionStorage.getItem('tokenKey');
        let role = 'Guest';
        if (item) {
            role = JSON.parse(item).role;
        }
        return role;
    },

    getToken: () => {
        let item = sessionStorage.getItem('tokenKey');
        let token = null;
        if (item) {
            token = JSON.parse(item).access_token;
        }
        return token;
    }
}