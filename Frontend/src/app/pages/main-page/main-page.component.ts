import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Route } from '@angular/router';
import { IFile } from 'src/app/models/file';
import { AuthService } from 'src/app/services/auth.service';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent implements OnInit {

  isLoading = true;
  files!: IFile[];

  constructor(private userService: UserService,
    private filesService: FilesService,
    private authService: AuthService,
    private snackbar: SnackbarService,
    private route: ActivatedRoute) {}

  ngOnInit(): void {
    let userId = this.route.snapshot.paramMap.get('id');

    if (userId == null) {
      if (this.authService.isUserLoggedIn()) {
        var userData = this.userService.getUserTokenData();
      }
      else {
        this.isLoading = false;
      }
    }


    this.filesService.getUserFilesMetadata(userData!.id).subscribe({
      next: (response) => {
        this.files = response;
        this.isLoading = false;
      },
      error: (error) => {
        this.snackbar.showMessage(error.message);
      }
    });
  }

  onFileUploaded(event : IFile) {
    console.log(event);
    this.files?.unshift(event);
  }

  onFilesDeleted(event : IFile[]) {
    this.files = this.files.filter(f => !event.includes(f));
  }

  onSearchPerformed(event : IFile[]) {
    this.files = event;
  }
}
