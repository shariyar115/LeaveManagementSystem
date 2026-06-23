import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api.config';
import { LeaveType, UpsertLeaveType } from '../models/leave.models';

@Injectable({ providedIn: 'root' })
export class LeaveTypeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);
  private get url() {
    return `${this.baseUrl}/leavetypes`;
  }

  getAll(): Observable<LeaveType[]> {
    return this.http.get<LeaveType[]>(this.url);
  }

  create(dto: UpsertLeaveType): Observable<LeaveType> {
    return this.http.post<LeaveType>(this.url, dto);
  }

  update(id: number, dto: UpsertLeaveType): Observable<LeaveType> {
    return this.http.put<LeaveType>(`${this.url}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
