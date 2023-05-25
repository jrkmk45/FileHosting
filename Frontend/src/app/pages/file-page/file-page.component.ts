import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { IFile } from 'src/app/models/file';
import { IUser } from 'src/app/models/users';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { UserService } from 'src/app/services/user.service';
import { FormatStringHelper } from 'src/app/utils/format-string-helper';

@Component({
  selector: 'app-file-page',
  templateUrl: './file-page.component.html',
  styleUrls: ['./file-page.component.css']
})
export class FilePageComponent implements OnInit {

  file?: IFile;
  notPermittedUsers?: IUser[];
  isLoading = true;

  selectedAccess!: number;

  constructor(private filesService: FilesService,
    private route: ActivatedRoute,
    private snackbar: SnackbarService,
    public formatHelper: FormatStringHelper,
    public userService: UserService,
    private router: Router) {}

  ngOnInit() {
    let fileId = this.route.snapshot.paramMap.get('id')!;
    this.filesService.getFileMetadata(fileId).subscribe({
      next: (file) => {
        this.file = file;
        this.selectedAccess = this.file.accessability!;
        this.isLoading = false;
      },
      error: (error: HttpErrorResponse) => {
        if (error.status == 401) {
          this.snackbar.showMessage("Авторизуйтесь щоб отримати доступ до цього файлу!");
        }
        else if (error.status == 403) {
          this.snackbar.showMessage("У вас немає доступу до цього файлу!");
        }
        this.router.navigate(['privacy-error']);
      }
    });

    this.userService.getNotPermittedUsersByFile(fileId).subscribe((users) => {
      this.notPermittedUsers = users;
    });
  }

  formatDate(date: string) {
    let dateTime = Date.parse(date);
    return dateTime.toString();
  }

  onDownloadClick() {
    this.filesService.downloadFile(this.file!.id);
  }

  onChangeAccessabilityClick() {
    let body = {
      accesability: this.selectedAccess
    }
    this.filesService.patchFile(this.file!.id, body).subscribe({
      next: () => {
        this.snackbar.showMessage("Доступ успішно оновлено!");
        this.file!.accessability=this.selectedAccess;
      },
      error: () => {
        this.snackbar.showMessage("Не вдалося оновити доступ");
      }
    });
  }

  onUserPermitted(event : IUser) {
    let permittedUserIds = this.file!.permittedUsers!.map(u => u.id)
    permittedUserIds.push(event.id);
    this.filesService.patchFile(this.file!.id, { permittedUsers: permittedUserIds }).subscribe({
      next: () => {
        this.file!.permittedUsers?.push(event);
        this.notPermittedUsers = this.notPermittedUsers?.filter(u => u.id != event.id);
        this.snackbar.showMessage("Успішно надано доступ!");
      },
      error: () => {
        this.snackbar.showMessage("Виникла помилка при наданні доступу");
      }
    });
  }

  onUserDepermitted(event : IUser) {
    let permittedUserIds = this.file!.permittedUsers!.map(u => u.id)
    console.log(permittedUserIds);
    permittedUserIds = permittedUserIds.filter(u => u != event.id);
    console.log(permittedUserIds);
    this.filesService.patchFile(this.file!.id, { permittedUsers: permittedUserIds }).subscribe({
      next: () => {
        this.file!.permittedUsers = this.file?.permittedUsers?.filter(u => u != event)
        
        this.notPermittedUsers?.push(event);
        this.snackbar.showMessage("Доступ забраний успішно!");
      },
      error: () => {
        this.snackbar.showMessage("Виникла помилка при забиранні доступу");
      }
    });
  }
}
