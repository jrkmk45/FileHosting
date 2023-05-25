import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IUser } from 'src/app/models/users';

@Component({
  selector: 'app-users-access-remove',
  templateUrl: './users-access-remove.component.html',
  styleUrls: ['./users-access-remove.component.css']
})
export class UsersAccessRemoveComponent {

  @Input() users? : IUser[];
  @Output() userDepermitted = new EventEmitter<IUser>();

  onDepermitClick(user: IUser) {
    this.userDepermitted.emit(user);
  }
}
