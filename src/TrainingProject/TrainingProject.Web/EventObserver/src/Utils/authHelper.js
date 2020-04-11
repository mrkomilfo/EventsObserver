export default {
    saveAuth: (id, token, role) => {
        sessionStorage.setItem('tokenKey', JSON.stringify({ userId: id, access_token: token, role: role }));
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