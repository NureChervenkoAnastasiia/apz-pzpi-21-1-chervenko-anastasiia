package com.example.tastifymobile.api

import com.example.tastifymobile.models.Guest
import com.example.tastifymobile.models.GuestRegistration
import com.example.tastifymobile.models.LoginRequest
import com.example.tastifymobile.models.LoginResponse
import com.example.tastifymobile.models.Menu
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface ApiService {
    @POST("api/Guest/login")
    fun login(@Body loginRequest: LoginRequest): Call<LoginResponse>

    @GET("api/Guest/{guestId}")
    fun getGuestById(@Path("guestId") guestId: String): Call<Guest>

    @POST("api/Guest/register")
    fun registerGuest(@Body guestRegistration: GuestRegistration): Call<LoginResponse>

    @GET("api/Menu")
    fun getAllDishes(): Call<List<Menu>>



}
