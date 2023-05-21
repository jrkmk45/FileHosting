import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SnackbarService {

  constructor(private snackbar: MatSnackBar) { }

  showMessage(message: string) {
    return this.snackbar.open(message, 'OK', {
      duration: 5000,
    });
  }
}
