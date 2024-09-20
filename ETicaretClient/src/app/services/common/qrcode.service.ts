import { Injectable } from '@angular/core';
import { HttpClientService } from './http-client.service';
import { firstValueFrom, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class QrcodeService {

  constructor(private httpClientService: HttpClientService) { }

  async generateQRCode(productId: string, successCallBack?: () => void, errorCallBack?: (error) => void){
    //Gelen data bir file olduğundan dolayı geri dönüş tipini Blob yapıyoruz.
    const observable: Observable<Blob> = this.httpClientService.get({
      controller: "products",
      action: "GetQRCodeToProduct",
      responseType: "blob"
    }, productId);

    const promiseData = firstValueFrom(observable);
    promiseData.then(successCallBack)
    .catch(errorCallBack);
    return await promiseData;
  }
}
