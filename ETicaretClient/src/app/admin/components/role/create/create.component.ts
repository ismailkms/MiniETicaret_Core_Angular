import { Component, EventEmitter, Output } from '@angular/core';
import { AlertifyService, MessageType, Position } from '../../../../services/admin/alertify.service';
import { BaseComponent, SpinnerType } from '../../../../base/base.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { RoleService } from '../../../../services/common/models/role.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateComponent extends BaseComponent {

  constructor(spinner: NgxSpinnerService, private roleService: RoleService, private alertify: AlertifyService) {
    super(spinner)
  }

  @Output() createdRole: EventEmitter<string> = new EventEmitter();

  create(name: HTMLInputElement) {
    this.showSpinner(SpinnerType.BallAtom);

    this.roleService.create(name.value, () => {
      this.hideSpinner(SpinnerType.BallAtom);
      this.alertify.message("Role başarıyla eklenmiştir.", {
        dismissOthers: true,
        messageType: MessageType.Success,
        position: Position.BottomRight
      });
      this.createdRole.emit(name.value);
    }, errorMessage => {
      this.alertify.message(errorMessage, {
        dismissOthers: true,
        messageType: MessageType.Error,
        position: Position.BottomRight
      });
    });
  }
}
