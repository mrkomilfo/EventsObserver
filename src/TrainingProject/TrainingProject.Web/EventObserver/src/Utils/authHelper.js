export default class AuthHelper {
    static saveAuth(id, role, accessToken, refreshToken) {
        localStorage.setItem('userId', id);
        localStorage.setItem('role', role);
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', refreshToken);
    }

    static clearAuth() {
        localStorage.removeItem('userId');
        localStorage.setItem('role', 'Guest');
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
    }

    static getId () {
        return localStorage.getItem('userId');
    }

    static getRole() {
        return localStorage.getItem('role') || 'Guest';
    }

    static getAccessToken() {
        return localStorage.getItem('accessToken');
    }

    static getRefreshToken() {
        return localStorage.getItem('refreshToken');
    }

    static saveAccessToken(accessToken) {
        localStorage.setItem('accessToken', accessToken);
    }

    static saveRefreshToken(refreshToken) {
        localStorage.setItem('refreshToken', refreshToken);
    }

    static async fetchWithCredentials(url, options) {
        const jwtToken = this.getAccessToken();
        
        options = options || {};
        options.headers = options.headers || {};
        options.headers['Authorization'] = 'Bearer ' + jwtToken;

        const response = await fetch(url, options);
        
        if (response.ok) { //all is good, return the response
            return response;
        }

        if (response.status === 401 && response.headers.has('Token-Expired')) {
            const refreshToken = this.getRefreshToken();
            const refreshResponse = await this.refresh(jwtToken, refreshToken);
            
            if (!refreshResponse.ok) {
                return response; //failed to refresh so return original 401 response
            }
            
            const jsonRefreshResponse = await refreshResponse.json(); //read the json with the new tokens
            
            this.saveAccessToken(jsonRefreshResponse.accessToken);
            this.saveRefreshToken(jsonRefreshResponse.refreshToken);
            
            return await this.fetchWithCredentials(url, options); //repeat the original request
        } else { //status is not 401 and/or there's no Token-Expired header
            return response; //return the original 401 response
        }
    }

    static async refresh(jwtToken, refreshToken) {
        let tokens = {
            token: jwtToken,
            refreshToken: refreshToken
        }
        return fetch('api/users/refresh', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(tokens)
        });
    }
}