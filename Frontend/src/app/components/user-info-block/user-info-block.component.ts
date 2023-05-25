import { Component, Input } from '@angular/core';
import { IUser } from 'src/app/models/users';

@Component({
  selector: 'app-user-info-block',
  templateUrl: './user-info-block.component.html',
  styleUrls: ['./user-info-block.component.css']
})
export class UserInfoBlockComponent {
  @Input() user? : IUser;
  @Input() showAvatar = true;
}
