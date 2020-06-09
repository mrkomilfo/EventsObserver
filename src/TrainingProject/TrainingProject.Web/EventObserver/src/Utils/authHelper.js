﻿export default class AuthHelper {
    static decryptJWT(token) {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    }

    static isTokenValid() {  
        const token = sessionStorage.getItem('tokenKey');
        if (!token)
            return false;
        if (JSON.parse(token).exp < (new Date()).getTime() / 1000) {
            this.clearAuth();
            return false;
        }
        return true;
    }

    static saveAuth(id, token, role, login, password) {
        const decrypted = this.decryptJWT(token);
        sessionStorage.setItem('tokenKey', JSON.stringify({ userId: id, access_token: token, role: role, login: login, password: password, exp: decrypted.exp }));
    }

    static clearAuth () {
        sessionStorage.removeItem('tokenKey');
    }

    static getId () {
        if (this.isTokenValid()) {
            const token = sessionStorage.getItem('tokenKey');
            return JSON.parse(token).userId;
        }
        return null;
    }

    static getRole() {
        if (this.isTokenValid()) {
            const token = sessionStorage.getItem('tokenKey');
            return JSON.parse(token).role;
        }
        return 'Guest';
    }

    static async refreshToken() {
        const token = sessionStorage.getItem('tokenKey');
        if (!token)
            return;
        const auth_data = {
            login: JSON.parse(token).login,
            password: JSON.parse(token).password
        };
        let error = false;
        await fetch('api/Users/signIn', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(auth_data)
        }).then(async (response) => {
            error = !response.ok;
            return await response.json();
        }).then((data) => {
            if (!error) {
                this.saveAuth(data.name, data.accessToken, data.role, auth_data.login, auth_data.password);
            }
            else {
                this.clearAuth();
            }
        }).catch((ex) => {
            this.clearAuth();
        });
    }

    static async getToken() {
        await this.refreshToken();
        if (this.isTokenValid()) {            
            const token = sessionStorage.getItem('tokenKey');
            if (!!token)
                return JSON.parse(token).access_token;
            this.clearAuth();
        }
        return null;
    }
}