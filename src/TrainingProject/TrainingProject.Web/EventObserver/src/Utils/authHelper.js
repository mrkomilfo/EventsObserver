export default class AuthHelper {
    static saveAuth(id, role, accessToken, refreshToken) {
        sessionStorage.setItem('userId', id);
        sessionStorage.setItem('role', role);
        sessionStorage.setItem('accessToken', accessToken);
        sessionStorage.setItem('refreshToken', refreshToken);
    }

    static clearAuth () {
        sessionStorage.removeItem('userId');
        sessionStorage.setItem('role', 'Guest');
        sessionStorage.removeItem('accessToken');
        sessionStorage.removeItem('refreshToken');
    }

    static getId () {
        return sessionStorage.getItem('userId');
    }

    static getRole() {
        return sessionStorage.getItem('role') || 'Guest';
    }

    static getAccessToken() {
        return sessionStorage.getItem('accessToken');
    }

    static getRefreshToken() {
        return sessionStorage.getItem('refreshToken');
    }

    static saveAccessToken(accessToken) {
        localStorage.setItem('accessToken', accessToken);
    }

    static saveRefreshToken(refreshToken) {
        localStorage.setItem('refreshToken', refreshToken);
    }

    static async fetchWithCredentials(url, options) {
        var jwtToken = this.getAccessToken();
        options = options || {};
        options.headers = options.headers || {};
        options.headers['Authorization'] = 'Bearer ' + jwtToken;
        var response = await fetch(url, options);
        if (response.ok) { //all is good, return the response
            return response;
        }

        if (response.status === 401 && response.headers.has('Token-Expired')) {
            var refreshToken = this.getRefreshToken();

            var refreshResponse = await this.refresh(jwtToken, refreshToken);
            if (!refreshResponse.ok) {
                return response; //failed to refresh so return original 401 response
            }
            var jsonRefreshResponse = await refreshResponse.json(); //read the json with the new tokens

            this.saveAccessToken(jsonRefreshResponse.accessToken);
            this.saveRefreshToken(jsonRefreshResponse.refreshToken);
            return await this.fetchWithCredentials(url, options); //repeat the original request
        } else { //status is not 401 and/or there's no Token-Expired header
            return response; //return the original 401 response
        }
    }

    static async refresh() {
        return fetch('api/Users/refresh', {
            method: 'POST',
            body: `token=${encodeURIComponent(this.getAccessToken())}&refreshToken=${encodeURIComponent(this.getRefreshToken())}`,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        });
    }
}