import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { SnackbarService } from 'src/app/services/snackbar.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  hide = true;
  submitPressed = false;

  constructor(private builder: FormBuilder,
    private authService: AuthService, 
    private snackbar: SnackbarService,
    private router: Router) {
  }

  loginForm = this.builder.group({
    userName: this.builder.control('', Validators.required),
    password: this.builder.control('', Validators.compose([Validators.required, Validators.minLength(5)]))
  });

  get userName() {
    return this.loginForm.get('userName');
  }

  get password() {
    return this.loginForm.get('password');
  }

  loginErrorMessage = null;
  proceedLogin() {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: (response) => {
          localStorage.setItem("token", response.token);
          this.authService.setIsLoggedIn(true);
          this.snackbar.showMessage("Успішний вхід!");
          this.router.navigateByUrl('');
        },
        error: (response) => {
          this.loginErrorMessage = response.error.message;
        }
      });
    }
  }
}
