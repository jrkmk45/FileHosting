import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {

  public showHamburger = false;
  
  constructor(private authService: AuthService, 
    public userService: UserService) { }

  public isLoggedIn?: boolean;

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe((value) => {
      this.isLoggedIn = value;
      this.userService.getMyInfo().subscribe();
    });

    this.isLoggedIn = this.authService.isUserLoggedIn();
    if (this.isLoggedIn) {
      this.userService.getMyInfo().subscribe();
    }
  }

  public logout() {
    this.authService.logout();
  }
}
