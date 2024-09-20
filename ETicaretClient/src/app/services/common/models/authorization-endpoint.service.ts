import { Injectable } from '@angular/core';
import { HttpClientService } from '../http-client.service';
import { firstValueFrom, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationEndpointService {

  constructor(private httpClientService: HttpClientService) { }

  async assignRoleEndpoint(roles: string[], code: string, menu: string, successCallBAck?: () => void, errorCallBack?: (error) => void) {
    const observable: Observable<any> = this.httpClientService.post({
      controller: "authorizationendpoints"
    }, {
      roles: roles,
      code: code,
      menu: menu
    });

    const promiseData = firstValueFrom(observable);
    promiseData.then(successCallBAck)
      .catch(errorCallBack);

    await promiseData;
  }

  async getRolesToEndpoint(code: string, menu: string, successCallBAck?: () => void, errorCallBack?: (error) => void) {
    const observable: Observable<any> = this.httpClientService.post({
      controller: "authorizationendpoints",
      action: "GetRolesToEndpoint"
    }, {
      code: code,
      menu: menu
    });

    const promiseData = firstValueFrom(observable);
    promiseData.then(successCallBAck)
      .catch(errorCallBack);

    return (await promiseData).roles;
  }
}
