import { Directive, ElementRef, EventEmitter, HostListener, Input, Output, Renderer2 } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { SpinnerType } from '../../base/base.component';
import { MatDialog } from '@angular/material/dialog';
import { DeleteDialogComponent, DeleteState } from '../../dialogs/delete-dialog/delete-dialog.component';
import { HttpClientService } from '../../services/common/http-client.service';
import { AlertifyService, MessageType, Position } from '../../services/admin/alertify.service';
import { HttpErrorResponse } from '@angular/common/http';
import { DialogService } from '../../services/common/dialog.service';

declare var $: any;

@Directive({
  selector: '[appDelete]'
})
export class DeleteDirective {

  constructor(
    private element: ElementRef,
    private _renderer: Renderer2,
    private httpClientService: HttpClientService,
    private spinner: NgxSpinnerService,
    public dialog: MatDialog,
    private alertifyService: AlertifyService,
    private dialogService: DialogService
  ) {
    const img = _renderer.createElement("img");
    img.setAttribute("src", "/assets/delete.png");
    img.setAttribute("style", "cursor:pointer;");
    img.width = 25;
    img.height = 25;
    _renderer.appendChild(element.nativeElement, img);
  }

  @Input() id: string; //Html elementi içerisine yazığımız [id]="element.id" yi yakalayıp bize getiriyor. Burada id adını biz verdik elment de değişebilir.
  @Input() controller: string;
  @Output() callback: EventEmitter<any> = new EventEmitter(); //Html elementi içerisine yazdığımız (callback)="getProducts()" çağırıyor. Burada callback ismini biz verdik. İçindeki method'u istediğim yerde tetikleyebileceğim.

  @HostListener("click") //HostListener, ilgili directive'in kullanıldığı html nesnesine tıklanıldığında " " içine verilen olay gerçekleştirildiğinde altına yazılan method(burada onclick yerine ahmet'te olabilir) tetiklenecektir.
  async onclick() {

    this.dialogService.openDialog({
      componentType: DeleteDialogComponent,
      data: DeleteState.Yes,
      afterClosed: () => {
        this.spinner.show(SpinnerType.BallAtom);
        const td: HTMLTableCellElement = this.element.nativeElement;
        this.httpClientService.delete({
          controller: this.controller,
          action: `${this.controller == 'products' ? 'delete' : ""}`
        }, this.id).subscribe(data => {
          $(td.parentElement).animate({
            opacity: 0,
            left: "+=50",
            height: "toogle"
          }, 700, () => {
            this.callback.emit();
            this.alertifyService.message(`${this.controller == 'roles' ? 'Rol' : 'Ürün'} başarıyla silinmiştir`, {
              messageType: MessageType.Success,
              position: Position.BottomRight,
              dismissOthers: true
            })
          });
        }, (errorResponse: HttpErrorResponse) => {
          this.spinner.hide(SpinnerType.BallAtom);
          this.alertifyService.message(`${this.controller == 'roles' ? 'Rol' : 'Ürün'} silinirken beklenmeyen bir hatayla karşılaşılmıştır.`, {
            messageType: MessageType.Error,
            position: Position.BottomRight,
            dismissOthers: true
          });
        });
      }
    });
  }
}
