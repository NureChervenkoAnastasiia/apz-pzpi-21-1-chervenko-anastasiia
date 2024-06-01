// ProfileActivity.kt
package com.example.tastifymobile.activities

import android.os.Bundle
import android.util.Log
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.auth0.android.jwt.JWT
import com.example.tastifymobile.R
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.api.TokenManager
import com.example.tastifymobile.models.Guest
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class ProfileActivity : AppCompatActivity() {

    private lateinit var tokenManager: TokenManager
    private lateinit var apiService: ApiService
    private lateinit var nameTextView: TextView
    private lateinit var emailTextView: TextView
    private lateinit var phoneTextView: TextView
    private lateinit var bonusTextView: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)

        nameTextView = findViewById(R.id.nameTextView)
        emailTextView = findViewById(R.id.emailTextView)
        phoneTextView = findViewById(R.id.phoneTextView)
        bonusTextView = findViewById(R.id.bonusTextView)

        tokenManager = TokenManager(this)
        apiService = NetworkModule.provideApiService(this)

        val token = tokenManager.getJwtToken()
        if (token != null) {
            val jwt = JWT(token)
            val guestId = jwt.getClaim("id").asString()

            if (guestId != null) {
                fetchGuestInfo(guestId)
            } else {
                Log.e("ProfileActivity", "No guest ID found in token")
            }
        } else {
            Log.e("ProfileActivity", "No token found")
        }
    }

    private fun fetchGuestInfo(guestId: String) {
        apiService.getGuestById(guestId).enqueue(object : Callback<Guest> {
            override fun onResponse(call: Call<Guest>, response: Response<Guest>) {
                if (response.isSuccessful) {
                    val guest = response.body()
                    if (guest != null) {
                        nameTextView.text = guest.name
                        emailTextView.text = guest.email
                        phoneTextView.text = guest.phone
                        bonusTextView.text = guest.bonus.toString()
                    } else {
                        Log.e("ProfileActivity", "Failed to retrieve guest data")
                    }
                } else {
                    Log.e("ProfileActivity", "Failed to retrieve guest data: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Guest>, t: Throwable) {
                Log.e("ProfileActivity", "Error fetching guest data: ${t.message}")
            }
        })
    }
}
