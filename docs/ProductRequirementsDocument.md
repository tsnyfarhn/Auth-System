# PRODUCT REQUIREMENTS DOCUMENT (PRD): User Management System
## Sistem Pengelolaan Pengguna (User Management System )

***Pemilik Produk:** Tsany Farhan

**Status:** Draft

**Tanggal Pembuatan:** 16 November 2026

**Target Rilis:** November 2026

---

## 1. Ringkasan (*Summary*)

The User Management System is a web-based application desgined to provide centralized management of user accounts, identitties, authentication, and access control.

The system aims to deliver a secure, scalable, and reliable foundation for managing user access accross various business applications, ensuring cosistent account management and steamlined access control within the organization's application system.

---

## 2. Latar Belakang & Tujuam (*Background & Objective*)
* **Latar Belakang:** Saat ini setiap akun yang telah dibuat tidak memiliki pengelolaan akun, dan sulit untuk memastikan data agar sudah sesuai mulai dari hak akses yang diberikan, perubahan *password* pada akun *user*, dan lainnya.
* **Tujuan:** Untuk membuat setiap *user* dapat mengelola akun sendiri dan juga admin dapat mengelola akun *user* yang sudah dibuat agar tetap sesuai data-data yang diberikan.

---

## 3. Target Pengguna (*User Target*)
| User | Situasi | Hasil |
| :--- | :--- | :--- |
| Pengguna Baru | Saat membuat akun baru dengan *email* dan *password* sendiri | Akun baru terbuat |
| Pengguna Baru | Saat melakukan verifikasi *email* dengan kode OTP | Alamat *email* terverifikasi |
| Pengguna Lama | Saat melakukan *login* dengan *email* dan *passwrod* | Masuk kedalam *dashboard* |
| Pengguna Lama | Saat melakukan lupa *password* dengan memasukan *email* | Membuat *password* baru |
| Pengguna Lama | Saat melakukan ubah *password* dengan memasukan *password* lama dan *password* baru | *password* terubah |
| Pengguna | Saat mengelola akun *user* sendiri seperti membaca, dan ubah | Data akun *user* sendiri berubah |
| Admin | Saat mengelola akun *user* seperti membaca, tambah, ubah, dan hapus | Data akun *user* berubah |
| Admin | Saat mengelola *user role* seperti membaca, tambah, ubah, dan hapus | Data *user role* berubah |
| Admin | Saat mengelola hak akses akun *user* seperti membaca, tambah, ubah, dan hapus | Data hak akses akun *user* berubah |

---

## 4. Persyaratan Fungsional (*Functional Requirements*)

### 4.1 Daftar (*Register*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | *User* dapat melakukan daftar menggunakan *email* dan *password* | P0 |
| FR-02 | Sistem akan memvalidasi apakah *email* sudah terdaftar | P0 |
| FR-03 | Sistem akan mengirimkan kode OTP untuk verifikasi *email* | P0 |
| FR-04 | *User* dapat meminta sistem untuk mengirimkan kode OTP ulang | P0 |
| FR-05 | *User* dapat memasukkan kode OTP yang sudah dikirimkan untuk verifikasi *email* | P0 |

### 4.2 Masuk (*Daftar*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | *User* dapat masuk kedalam *dashboard* menggunakan *email* dan *password* sendiri | P0 |
| FR-02 | Sistem akan melakukan validasi *email* dan *password* saat proses *login* | P0 |
| FR-03 | *User* dapat tidak melakukan proses *login* ulang jika sebelumnya sudah *login* | P0 |
| FR-04 | *User* dapat masuk kedalam *dashboard* dengan berbagai perangkat | P1 |

### 4.3 Keluar (*Logout*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | *User* dapat keluar dari *dashboard* | P0 |
| FR-02 | Sistem akan membuat user keluar otomatis jika tidak ada aktivitas selama 1 jam | P0 |
| FR-03 | *User* dapat keluar dari semua perangkat yang sudah masuk sebelumnya | P1 |

### 4.4 Lupa Kata Sandi (*Forgot Password*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | *User* dapat melakukan proses *forgot password* dengan memasukkan *email* | P0 |
| FR-02 | Sistem akan melakukan validasi apakah *email* tersebut sudah terdaftar | P0 |
| FR-03 | Sistem akan mengirimkan *link* untuk merubah *password* | P0 |
| FR-04 | *User* dapat merubah *password* yang baru | P0 |
| FR-05 | *User* dapat meminta sistem untuk mengirimkan ulang *link* untuk merubah *password* | P0 |

### 4.5 Ubah Kata Sandi (*Change Password*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | *User* dapat merubah *password* lama dengan memasukkan *password baru* | P0 |
| FR-02 | Sistem akan memeriksa apakah *password* baru tidak sama dengan *password* lama | P0 |

### 4.6 Mengelola Akun Pengguna (*User Account Management*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | *User* dapat melihat data akun sendiri | P0 |
| FR-02 | Admin dapat melihat data akun keseluruhan yang sudah terdaftar | P0 |
| FR-03 | Admin dapat merubah data akun keseluruhan yang sudah terdaftar | P0 |
| FR-04 | Admin dapat menhapus data akun keseluruhan yang sudah terdaftar | P0 |

### 4.7 Mengelola Peran (*Role Management*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | Admin dapat melihat data *role* keseluruhan yang ada dan *user* mana saja yang memiliki role tersebut | P0 |
| FR-02 | Admin dapat menambahkan data *role* untuk setiap *user* | P0 |
| FR-03 | Admin dapat merubah data *role* untuk setiap *user* | P0 |
| FR-04 | Admin dapat menghapus data *role* untuk setiap *user* | P0 |

### 4.8 Mengelola hak akses (*Permission Management*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| FR-01 | Admin dapat melihat data hak akses keseluruhan yang ada dan *role* mana saja yang memiliki hak akses tersebut | P0 |
| FR-02 | Admin dapat merubah data hak akses untuk setiap *role* | P0 |

---

## 5. Persyaratan Non-Fungsional (*Non-Functional Requirements*)

### 5.1 Keamanan (*Security*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| NFR-01 | Membatasi percobaan login sebanyak 5 kali dalam 5 menit untuk menghindari *brute-force attempts* | P0 |
| NFR-02 | Sistem akan mengirimkan JWT *access token & refresh token* untuk setiap kali login | P0 |
| NFR-03 | Sistem akan melakukan *refresh token* setiap 15 menit | P0 |
| NFR-04 | *Token* akan *expired* dalam 24 jam | P0 |
| NFR-05 | *Password* harus dienkripsi menggunakan *hashing* dan tidak boleh disimpan dalam bentuk *plain text* | P0 |
| NFR-06 | *Password* harus lebih dari 8 karakter dengan huruf dan angka | P0 |
| NFR-01 | Membatasi pengiriman ulang kode OTP selama 1 menit | P0 |
| NFR-07 | Kode OTP akan *expired* apabila sudah lebih dari 15 menit | P0 |
| NFR-08 | Melakukan konfirmasi ulang *password* untuk proses ubah *password* dan *forgot password* | P0 |
| NFR-09 | *Link* untuk *forgot password* akan *expired* apabila sudah lebih dari 15 menit | P0 |

### 5.2 Performa (*Performance*)
| ID | Requirement | Prioritas |
| :--- | :--- | :--- |
| NFR-01 | Waktu respons untuk proses *login, register, verify email, change password, reset password* harus dibawah 2 detik | P0 |
| NFR-01 | Sistem harus mampu menangani hingga 10.000 permintaan *login* secara berbarengan per menit | P0 |
| NFR-01 | *Email forgot password* dan *Verify email* harus terkirim dalam waktu maksimal 30 detik | P0 |

## 6. Alur Pengguna (*User Flow*)
* **Login:** Halaman Login -> Input Email & Password -> [Tombol Login] -> Validasi Sistem -> [Masuk ke halaman Dashboard]
* **Register:** Halaman Login -> [Link Register] -> Halaman Register -> Input Email & Password -> [Tombol Register] -> Halaman Verifikasi Email -> Input Kode OTP -> [Tombol Kirim] -> Validasi Sistem -> Halaman Login
* **Logout:** Halaman Dashboard -> Icon Akun -> [Tombol Logout] -> Validasi Sistem -> Halaman Login
* **Forgot Password:** Halaman Login -> [Link Forgot Password] -> Dikirim ke email user -> [Link Reset Password] -> Halaman Reset Password -> Input Password Baru -> [Tombol Reset Password] -> Validasi Sistem -> Halaman Login
* **Change Password:** Halaman Dashboard -> Menu Pengaturan Akun -> Halaman Pengelola Akun -> [Tombol Ubah Password] -> Form Modal Ubah Password -> Input Password lama dan baru -> [Tombol Simpan] -> Validasi Sistem -> Halaman Pengaturan akun
* **Edit User:** Halaman Dahsboard -> Menu Pengelola User -> Halaman Pengelola User -> [Tombol Ubah User] -> Form Modal Ubah User -> Input Data -> [Tombol Simpan] -> Validasi Sistem -> Halaman Pengelola User
* **Delete User:** Halaman Dahsboard -> Menu Pengelola User -> Halaman Pengelola User -> [Tombol Hapus User] -> Form Modal Hapus User -> [Tombol Ya] -> Validasi Sistem -> Halaman Pengelola User
* **Add New Role:** Halaman Dahsboard -> Menu Pengelola Role -> Halaman Pengelola Role -> [Tombol Tambah Role] -> Form Modal Tambah Role -> Input Data -> [Tombol Simpan] -> Validasi Sistem -> Halaman Pengelola Role
* **Edit Role:** Halaman Dahsboard -> Menu Pengelola Role -> Halaman Pengelola Role -> [Tombol Ubah Role] -> Form Modal Ubah Role -> Input Data -> [Tombol Simpan] -> Validasi Sistem -> Halaman Pengelola Role
* **Delete Role:** Halaman Dahsboard -> Menu Pengelola Role -> Halaman Pengelola Role -> [Tombol Ubah Role] -> Form Modal Hapus Role -> [Tombol Ya] -> Validasi Sistem -> Halaman Pengelola Role
* **Edit Permission:** Halaman Dahsboard -> Menu Pengelola Hak Akses -> Halaman Pengelola Hak Akses -> [Pilih Chekcbox Hak Akses] -> [Tombol Simpan] -> Validasi Sistem