import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IFile } from 'src/app/models/file';
import { IUser } from 'src/app/models/users';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-files-page',
  templateUrl: './user-files-page.component.html',
  styleUrls: ['./user-files-page.component.css']
})
export class UserFilesPageComponent implements OnInit {

  files?: IFile[];
  user?: IUser;

  constructor(private usersService: UserService,
    private filesService: FilesService,
    private route: ActivatedRoute,
    private snackbar: SnackbarService) {}

  ngOnInit() {
    let userId = +this.route.snapshot.paramMap.get('id')!;

    this.usersService.getUserById(userId).subscribe({
      next: (user) => {
        this.user = user;
      },
      error: () => {
        this.snackbar.showMessage("Сталася помилка при завантаженні користувача");
      }
    });

    this.filesService.queryUserFiles(userId).subscribe({
      next: (files) => {
        this.files = files;
      },
      error: () => {
        this.snackbar.showMessage("Сталася помилка при завантаженні файлів");
      }
    });
  }

}
