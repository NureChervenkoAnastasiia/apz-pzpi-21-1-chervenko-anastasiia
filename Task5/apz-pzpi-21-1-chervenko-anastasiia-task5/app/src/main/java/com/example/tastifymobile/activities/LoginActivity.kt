package com.example.tastifymobile.activities

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.tastifymobile.R
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.api.TokenManager
import com.example.tastifymobile.models.LoginRequest
import com.example.tastifymobile.models.LoginResponse
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class LoginActivity : AppCompatActivity() {

    private lateinit var loginEditText: EditText
    private lateinit var passwordEditText: EditText
    private lateinit var loginButton: Button

    private lateinit var apiService: ApiService
    private lateinit var tokenManager: TokenManager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        loginEditText = findViewById(R.id.loginEditText)
        passwordEditText = findViewById(R.id.passwordEditText)
        loginButton = findViewById(R.id.loginButton)

        tokenManager = TokenManager(this)
        apiService = NetworkModule.provideApiService(this)

        loginButton.setOnClickListener {
            performLogin()
        }
    }

    private fun performLogin() {
        val login = loginEditText.text.toString()
        val password = passwordEditText.text.toString()

        if (login.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, "Email and password are required", Toast.LENGTH_SHORT).show()
            return
        }

        val loginRequest = LoginRequest(login, password)
        apiService.login(loginRequest).enqueue(object : Callback<LoginResponse> {
            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
                if (response.isSuccessful) {
                    val token = response.body()?.token
                    if (token != null) {
                        // Зберегти токен у TokenManager
                        tokenManager.saveJwtToken(token)

                        // Перейти до ProfileActivity
                        val intent = Intent(this@LoginActivity, ProfileActivity::class.java)
                        startActivity(intent)
                        finish()
                    } else {
                        Toast.makeText(this@LoginActivity, "Failed to retrieve token", Toast.LENGTH_SHORT).show()
                    }
                } else {
                    Toast.makeText(this@LoginActivity, "Login failed", Toast.LENGTH_SHORT).show()
                }
            }

            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                Log.e("LoginActivity", "Login error: ${t.message}")
                Toast.makeText(this@LoginActivity, "Login error", Toast.LENGTH_SHORT).show()
            }
        })
    }
}
