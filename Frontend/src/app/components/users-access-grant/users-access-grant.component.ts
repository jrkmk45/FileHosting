import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IUser } from 'src/app/models/users';

@Component({
  selector: 'app-users-access-grant',
  templateUrl: './users-access-grant.component.html',
  styleUrls: ['./users-access-grant.component.css']
})
export class UsersAccessGrantComponent {

  @Input() users? : IUser[];
  @Output() userPermitted = new EventEmitter<IUser>();

  onGrantAccessClick(user: IUser) {
    this.userPermitted.emit(user);
  }
}
