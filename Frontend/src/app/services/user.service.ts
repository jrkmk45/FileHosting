import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { API_URL } from '../constants';
import { HttpClient } from '@angular/common/http';
import { IUser } from '../models/user';
import jwt_decode from "jwt-decode";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  public userUpdated = new Subject();

  private _user! : IUser;

  get user() {
    return this._user;
  }

  constructor(private http: HttpClient) { }

  getMyInfo() : Observable<IUser> {
    return this.http.get<IUser>(API_URL + "users/me").pipe(
      tap(userResponse => this._user = userResponse)
    )
  }

  updateUser(user : any) {
    return this.http.put(API_URL + 'users/me', user).pipe(
      tap(response => {
        this.userUpdated.next(user);
      })
    );
  }



  public getUserTokenData() {
    if (localStorage.getItem("token") == null) {
      return null;
    }

    let token = localStorage.getItem('token');
    let decodedToken : any = jwt_decode(token!);

    let userData = {
      userName: decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      id: decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
    };
    return userData;
  }
}


