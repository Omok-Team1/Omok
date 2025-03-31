var express = require('express');
var router = express.Router();

// password 암호화
var bcrypt = require('bcrypt');
const { ObjectId } = require('mongodb');
var saltrounds = 10;

// ================= 업로드 처리 ==================
// 이미지 업로드/전송을 위한 multer 모듈 
const multer = require('multer');

// 프로필 이미지를 위한 경로 설정
var path = require('path');
const profilePath = path.join(__dirname, 'profiles');
router.use(express.static(profilePath));

// 프로필 이미지 업로드 설정
var storage = multer.diskStorage({
  // 이미지 파일 저장되는 위치
  destination: function(req, file, cb){
    cb(null, profilePath);
  },
  // 파일 이름 + 업로드 일시
  filename: function(req, file, cb){
    const ext = path.extname(file.originalname);
    const basename = path.basename(file.originalname, ext);
    cb(null, basename
            + '-' + Date.now() 
            + ext
    ); 
  },
  // 파일 type 제한
  fileFilter: function(req, file, callback){
    var ext = path.extname(file.originalname);
    if(ext!==".png" && ext!==".jpg" && ext !== ".jpeg"){
      return callback(new Error("PNG, JPG만 업로드 가능합니다"));
    }
    callback(null, true);
  },
  // 사이즈 제한
  limits: {
    filesize: 1024*1024, 
  },
});

const upload = multer({storage: storage});

// 프로필 이미지 파일 업로드
router.post("/profileImageUpload", upload.single("profile"), function (req, res) {
  res.json({ fileName: req.file.filename});
});

// 요청 시, 본인 계정의 프로필 이미지 파일 전송
router.post("/getMyProfileImage", async (req, res) => {
  // 로그인 중일 경우에만 동작
  if (req.session.isAuthenticated) {
    res.sendFile(path.join(profilePath, req.session.profile));
  } else{
    res.send("로그인 필요");
  }
});

// 요청 시, 해당 경로의 프로필 이미지 파일 전송
router.post("/getProfileImage", async (req, res) => {
  res.sendFile(path.join(profilePath, req.body.filename));
});
// =================================================

//로그인 요청에 대한 응답 종류
var ResponseType = {
  INVALID_LOGIN: 0,
  SUCCESS: 1,
  TOO_MANY_ATTEMPTS: 2,
  DUPLICATE_LOGIN: 3
}

/* GET users listing. */
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});

// 랜덤 문자열 생성 함수
function MakeRandomString(length) {
  let result = '';
  const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  const charactersLength = characters.length;
  let counter = 0;
  while (counter < length) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
    counter += 1;
  }
  return result;
}

// 회원가입
router.post('/signup', async function(req,res,next){
  try {
    var username = req.body.username;
    var password = req.body.password;
    var nickname = req.body.nickname;
    var profile  = req.body.profile; // 프로필 이미지 파일 이름

    // 모든 입력이 완료되었는지 체크
    if(!username || !password || !nickname){
      return res.status(400).send("모든 필드를 입력해주세요");
    }

    // 아이디가 email 형식인지 확인
    let regEmail = /^[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*\.[a-zA-Z]{2,3}$/i;
    if(regEmail.test(username) == false){
      console.log("정규식 x");
      return res.status(400).send("아이디는 이메일 형식이어야 합니다");
    }

    // 이미 존재하는 사용자인지 체크
    var database = req.app.get('database');
    var users = database.collection('users');

    const existingUser = await users.findOne({username: username});
    if(existingUser){
      return res.status(409).send("이미 존재하는 사용자입니다");
    }

    // 비밀번호 암호화
    var salt = bcrypt.genSaltSync(saltrounds);
    var hash = bcrypt.hashSync(password, salt);

    // DB에 저장
    await users.insertOne({
      username: username,
      password: hash,
      nickname: nickname,
      profile:  profile, //프로필 이미지 이름
      rating: 250,
      win: 0,
      lose: 0,
      coin: 0,
      created_at: Date.now(),
      updated_at: Date.now(),
      login_banned_until: 0,
      logged_in: false,
      login_failed: 0
    });

    res.status(201).send("회원가입 되셨습니다");
  } catch(err){
    console.error("회원가입 도중 오류가 발생했습니다:",err);
    res.status(500).send("서버 오류가 발생했습니다");
  }
});

// 로그인
router.post("/signin", async function(req, res, next) {
  try {
    // 현재 로그인되지 않은 상태일 경우에만 동작
    if (!req.session.isAuthenticated) {
      var username = req.body.username;
      var password = req.body.password;
  
      // 모든 입력이 완료되었는지 체크
      if (!username || !password) {
        return res.status(400).send("모든 필드를 입력해주세요.");
      }
  
      // DB값과 비교
      var database = req.app.get('database');
      var users = database.collection('users');

      const existingUser = await users.findOne({ username: username });
      if (existingUser) {
        // 현재 로그인 밴 당한 상태가 아니라면
        if(existingUser.login_banned_until - Date.now() < 0){
          var compareResult = bcrypt.compareSync(password, existingUser.password);
          // 로그인 성공
          if (compareResult) {
            // 중복 로그인 체크
            if(existingUser.logged_in == true){
              // 중복 로그인인 경우 DULICATE_LOGIN(3)로 응답
              res.json({ result: ResponseType.DUPLICATE_LOGIN });
            } else{
              // 로그인 실패 횟수 초기화
              if(existingUser.login_failed > 0) {
                await users.updateOne(
                  { username: username },
                  { login_failed: 0 }
                );
              }
              // 로그인 상태 변경
              await users.updateOne(
                {username: username},
                {$set: {logged_in: true } }
              );
              // 세션 등록
              req.session.isAuthenticated = true;
              req.session.userId = existingUser._id.toString();
              req.session.username = existingUser.username;
              req.session.nickname = existingUser.nickname;
              req.session.profile = existingUser.profile;
              res.json({ result: ResponseType.SUCCESS });
            }
          } else {
            // 로그인 실패
            res.json({ result: ResponseType.INVALID_LOGIN });
            
            // 로그인 실패 횟수 +1
            await users.updateOne(
              {username: username},
              {$set: {login_failed: existingUser.login_failed+1 } }
            );

            // 5번 실패 시, 30분 간 로그인 불가
            if(existingUser.login_failed+1 >= 5) {
              // 로그인 실패 횟수 초기화하고 30분간 밴
              await users.updateOne(
                {username: username},
                {$set: { login_failed: 0, login_banned_until: Date.now() + 30 * 60 * 1000 } }
              );
            }
          }
        } else {
          res.json({ result: ResponseType.TOO_MANY_ATTEMPTS });
        }
      } else {
        res.json({ result: ResponseType.INVALID_LOGIN });
      }
    }
    else{
      res.send("이미 로그인 중입니다");
    }
  } catch (err) {
    console.error("로그인 중 오류 발생.", err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

// 로그아웃
router.post('/signout', function(req, res, next) {
  // 현재 로그인 중일 경우에만 동작
  if(req.session.isAuthenticated){
    
    // 로그인 상태 변경
    var database = req.app.get('database');
    var users = database.collection('users');
    users.updateOne(
      { _id: new ObjectId(req.session.userId) },
      { $set: {logged_in: false } }
    );

    // 세션 파괴
    req.session.destroy((err) => {
      if (err) {
        console.log("로그아웃 중 오류 발생");
        return res.status(500).send("서버 오류가 발생했습니다");
      }
      res.status(200).send("로그아웃 되었습니다");
    });
  }
});

// 탈퇴
router.post('/delete', async function(req, res, next) {
  try{
    // 현재 로그인 중일 경우에만 동작
    if(req.session.isAuthenticated){
      var userId = req.session.userId;
      // 세션 파괴
      req.session.destroy((err) => {
        if (err) {
          console.log("로그아웃 중 오류 발생");
          return res.status(500).send("서버 오류가 발생했습니다");
        }
      });
      // DB에서 해당 유저 데이터 삭제
      var database = req.app.get('database');
      var users = database.collection('users');
      await users.deleteOne(
        { _id: new ObjectId(userId) }
      );
      res.status(200).send("탈퇴 되었습니다");
    }
  } catch(err){
    console.error("탈퇴 도중 오류가 발생했습니다:",err);
    res.status(500).send("서버 오류가 발생했습니다");
  }
});

/*
// 랜덤문자열 생성 및 인증
router.post('/auth',function(req, res, next)){
  const auth = MakeRandomString(4);

}

// 비밀번호 변경
router.post('/init_pass', async function(req, res, next) {
  try {
    // 비밀번호 암호화
    var salt = bcrypt.genSaltSync(saltrounds);
    var hash = bcrypt.hashSync(req.body.password, salt);

    await users.updateOne(
      { username: req.body.username },
      { $set: {password: hash } }
    );
  } catch(err){
    console.error("비밀번호 변경 도중 오류가 발생했습니다:",err);
    res.status(500).send("서버 오류가 발생했습니다");
  }
});
*/

// rating 추가
router.post('/addscore', async function(req, res, next) {
  try {
    if (!req.session.isAuthenticated) {
      return res.status(400).send("로그인이 필요합니다.");
    }
    var userId = req.session.userId;
    var score = req.body.score;

    // 점수 유효성 검사
    if (!score || isNaN(score)) {
      return res.status(400).send("유효한 점수를 입력해주세요.");
    }

    var database = req.app.get('database');
    var users = database.collection('users');
    
    const user = await users.findOne({ _id: new ObjectId(userId) });
    if (!user) {
      return res.status(404).send("사용자를 찾을 수 없습니다.");
    }
    
    const result = await users.updateOne(
      { _id: new ObjectId(userId) },
      {
        $set: {
          rating: user.rating + Number(score),
          updatedAt: new Date()
        }
      }
    );
    res.status(200).json({ message: "점수가 성공적으로 업데이트 되었습니다." });  
  } catch (err) {
    console.error("점수 추가 중 오류 발생: ", err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

// rating 조회
router.get('/score', async function(req, res, next) {
  try {
    if (!req.session.isAuthenticated) {
      return res.status(403).send("로그인이 필요합니다.");
    }

    var userId = req.session.userId;
    var database = req.app.get('database');
    var users = database.collection('users');

    const user = await users.findOne({ _id: new ObjectId(userId) });

    if (!user) {
      return res.status(404).send("사용자를 찾을 수 없습니다.");
    }

    res.json({
      id: user._id.toString(),
      username: user.username,
      nickname: user.nickname,
      rating: user.rating || 0
    });
  } catch (err) {
    console.error("점수 조회 중 오류 발생: ", err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

module.exports = router;