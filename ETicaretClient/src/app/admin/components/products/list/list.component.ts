import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { List_Product } from '../../../../contracts/list_products';
import { ProductService } from '../../../../services/common/models/product.service';
import { BaseComponent, SpinnerType } from '../../../../base/base.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { AlertifyService, MessageType, Position } from '../../../../services/admin/alertify.service';
import { MatPaginator } from '@angular/material/paginator';
import { DialogService } from '../../../../services/common/dialog.service';
import { SelectProductImageDialogComponent } from '../../../../dialogs/select-product-image-dialog/select-product-image-dialog.component';
import { QrcodeDialogComponent } from '../../../../dialogs/qrcode-dialog/qrcode-dialog.component';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent extends BaseComponent implements OnInit {

  constructor(spinner: NgxSpinnerService, 
    private productService: ProductService, 
    private alertifyService: AlertifyService,
    private dialogService: DialogService) {
    super(spinner)
  }

  displayedColumns: string[] = ['name', 'stock', 'price', 'createdDate', 'updatedDate', 'photos', 'qrcode', 'edit', 'delete'];
  dataSource: MatTableDataSource<List_Product> = null;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  async getProducts() {
    this.showSpinner(SpinnerType.BallAtom);
    const allProducts: { totalProductCount: number, products: List_Product[] } = await this.productService.read(this.paginator ? this.paginator.pageIndex : 0, this.paginator ? this.paginator.pageSize : 5, () => this.hideSpinner(SpinnerType.BallAtom), errorMessage => this.alertifyService.message(errorMessage, {
      messageType: MessageType.Error,
      dismissOthers: true,
      position: Position.BottomRight
    }));

    this.dataSource = new MatTableDataSource<List_Product>(allProducts.products);
    this.paginator.length = allProducts.totalProductCount;
  }

  addProductImages(id: string){
    this.dialogService.openDialog({
      componentType: SelectProductImageDialogComponent,
      data: id,
      options: {
        width: "1400px"
      }
    });
  }

  showQRCode(productId: string) {
    this.dialogService.openDialog({
      componentType: QrcodeDialogComponent,
      data: productId
    });
  }

  async pageChanged() {
    await this.getProducts();
  }

  async ngOnInit() {
    await this.getProducts();
  }

}
