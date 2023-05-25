import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from "@angular/material/card";
import { UploadFileComponent } from './components/upload-file/upload-file.component';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileListComponent } from './components/file-list/file-list.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { TokenInterceptor } from './interceptors/token.interceptor';
import { FilePageComponent } from './pages/file-page/file-page.component';
import { ListFileComponent } from './components/list-file/list-file.component';
import { UserInfoBlockComponent } from './components/user-info-block/user-info-block.component';
import { UploadingFilesListComponent } from './components/upload-file/uploading-files-list/uploading-files-list.component';
import { FileIconComponent } from './components/file-icon/file-icon.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PublicFilesPageComponent } from './pages/public-files-page/public-files-page.component';
import { ControlBarComponent } from './components/control-bar/control-bar.component';
import { SearchBarComponent } from './components/search-bar/search-bar.component';
import { UserAvatarComponent } from './components/user-avatar/user-avatar.component';
import { UserSettingsPageComponent } from './pages/user-settings-page/user-settings-page.component';
import { MatRadioModule } from '@angular/material/radio';
import { PrivateFileErrorPageComponent } from './pages/private-file-error-page/private-file-error-page.component';
import { MatSelectModule } from '@angular/material/select';
import { UserFilesPageComponent } from './pages/user-files-page/user-files-page.component';
import { UsersAccessRemoveComponent } from './components/users-access-remove/users-access-remove.component';
import { UsersAccessGrantComponent } from './components/users-access-grant/users-access-grant.component';


@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    UploadFileComponent,
    MainPageComponent,
    FileListComponent,
    LoginComponent,
    RegisterComponent,
    RegisterPageComponent,
    LoginPageComponent,
    FilePageComponent,
    ListFileComponent,
    UserInfoBlockComponent,
    UploadingFilesListComponent,
    FileIconComponent,
    PublicFilesPageComponent,
    ControlBarComponent,
    SearchBarComponent,
    UserAvatarComponent,
    UserSettingsPageComponent,
    PrivateFileErrorPageComponent,
    UserFilesPageComponent,
    UsersAccessRemoveComponent,
    UsersAccessGrantComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatButtonModule,
    MatCardModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    MatSnackBarModule,
    BrowserAnimationsModule,
    MatInputModule,
    MatIconModule,
    MatFormFieldModule,
    MatCheckboxModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatSelectModule
  ],
  providers: [ 
    {
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptor,
    multi: true
    }, 
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
