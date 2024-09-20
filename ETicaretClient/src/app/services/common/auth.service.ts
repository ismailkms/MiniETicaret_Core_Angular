import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private jwtHelper: JwtHelperService) { }

  identityCheck() {
    if (typeof window !== 'undefined' && window.localStorage) {
      const token: string = localStorage.getItem("accessToken");
      //const decodeToken = jwtHelper.decodeToken(token);
      //const expirationDate: Date = jwtHelper.getTokenExpirationDate(token);

      let expired: boolean;
      try {
        expired = this.jwtHelper.isTokenExpired(token);
      } catch {
        expired = true;
      }

      _isAuthenticated: _isAuthenticated = token != null && !expired;
    }
  }

  get isAuthenticated(): boolean {
    return _isAuthenticated;
  }
}

export let _isAuthenticated: boolean;