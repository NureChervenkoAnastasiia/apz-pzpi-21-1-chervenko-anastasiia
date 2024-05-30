document.getElementById('loginForm').addEventListener('submit', async function(event) {
  event.preventDefault();

  const login = document.getElementById('login').value;
  const password = document.getElementById('password').value;

  try {
      const response = await fetch('https://localhost:7206/api/Staff/login', {
          method: 'POST',
          headers: {
              'Content-Type': 'application/json'
          },
          body: JSON.stringify({ login: login, password: password })
      });

      if (response.ok) {
          const data = await response.json();
          if (data && data.token) {
              localStorage.setItem('token', data.token);

              const decodedToken = jwt_decode(data.token);
              console.log('Decoded Token:', decodedToken);
              
              localStorage.setItem('userData', JSON.stringify(decodedToken));


              if (decodedToken.role === 'admin') {
                console.log('hello admin');
                  window.location.href = './admin/MenuPage.html';
              } else {
                console.log('hello staff');
                  window.location.href = './staff/ProfilePage.html';
              }
          } else {
              console.error('Error: Token not found in response');
          }
      } else {
          const error = await response.text();
          console.error('Error:', error);
      }
  } catch (error) {
      console.error('Error:', error.message);
  }
});