import { Injectable } from '@angular/core';
import { HttpClientService } from '../http-client.service';
import { Create_Product } from '../../../contracts/create_product';
import { HttpErrorResponse } from '@angular/common/http';
import { List_Product } from '../../../contracts/list_products';
import { firstValueFrom, Observable } from 'rxjs';
import { List_Product_Image } from '../../../contracts/list_product_image';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private httpClientService: HttpClientService) { }

  create(product: Create_Product, successCallBack?: () => void, errorCallBack?: (errorMessage: string) => void) {
    this.httpClientService.post({
      controller: "Products",
      action: "Post"
    }, product)
      .subscribe(result => {
        successCallBack();
      }, (errorResponse: HttpErrorResponse) => {
        const _error: Array<{ key: string, value: Array<string> }> = errorResponse.error;
        let message = "";
        _error.forEach((v, index) => {
          v.value.forEach((_v, _index) => {
            message += `${_v}<br>`;
          });
        });
        errorCallBack(message);
      });
  }

  async read(page: number = 0, size: number = 5, successCallBack?: () => void, errorCallBack?: (errorMessage: string) => void): Promise<{ totalProductCount: number, products: List_Product[] }> {
    const promiseData: Promise<{ totalProductCount: number, products: List_Product[] }> = this.httpClientService.get<{ totalProductCount: number, products: List_Product[] }>({
      controller: "products",
      action: "get",
      queryString: `page=${page}&size=${size}`
    }).toPromise(); //toPromise .net deki Task olarak düşünülebilir.

    promiseData.then(d => successCallBack())
      .catch((errorResponse: HttpErrorResponse) => errorCallBack(errorResponse.message));

    return await promiseData;
  }

  async delete(id: string) {
    const deleteObservable: Observable<any> = this.httpClientService.delete<any>({
      controller: "Products",
      action: "delete"
    }, id);

    await firstValueFrom(deleteObservable);
  }

  async readImages(id: string, successCallBack?: () => void): Promise<List_Product_Image[]> {
    const getObservable: Observable<List_Product_Image[]> = this.httpClientService.get<List_Product_Image[]>({
      controller: "products",
      action: "GetProductImages"
    }, id);

    const images: List_Product_Image[] = await firstValueFrom(getObservable);
    successCallBack();
    return images;
  }

  async deleteImage(id: string, imageId: string, successCallBack?: () => void) {
    const deleteObservable = this.httpClientService.delete({
      controller: "Products",
      action: "DeleteProductImage",
      queryString: `imageId=${imageId}`
    }, id);
    await firstValueFrom(deleteObservable);
    successCallBack();
  }

  async changeShowCaseImage(imageId: string, productId: string, successCallBack?: () => void): Promise<void> {
    const changeShowCaseImageObservable = this.httpClientService.get({
      controller: "products",
      action: "ChangeShowCaseImage",
      queryString: `imageId=${imageId}&productId=${productId}`
    });
    await firstValueFrom(changeShowCaseImageObservable);
    successCallBack();
  }

  async updateStockQrCodeToProduct(productId: string, stock: number, successCallBack?: () => void) {
    const observable: Observable<any> = this.httpClientService.put({
      controller: "products",
      action: "UpdateStockQrCodeToProduct"
    }, {
      productId,
      stock
    });

    await firstValueFrom(observable);
    successCallBack();
  }
}
