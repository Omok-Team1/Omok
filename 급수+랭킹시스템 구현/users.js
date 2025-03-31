var express = require('express');
var router = express.Router();
var bcrypt = require('bcrypt'); // 암호화를 위한 변수 선언
const { ObjectId } = require('mongodb');
var saltrounds = 10; // 암호화 반복 횟수  

var ResponseType = {
  INVALID_USEREMAIL: 0,
  INVALID_PASSWORD: 1,
  SUCCESS: 2
}

// 급수 시스템 설정
// 10급 ~ 18급 : 3점
// 5급 ~ 9급 : 5점
// 1급 ~ 4급 : 10점점
const rankSystem = [
  { min: 10, max: 18, pointsToRankUp: 3 },
  { min: 5, max: 9, pointsToRankUp: 5 },
  { min: 1, max: 4, pointsToRankUp: 10 }
]

// 이메일 유효성 검사 함수
function isValidEmail(useremail) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(useremail);
}

/* GET users listing. */
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});


// 회원가입
router.post('/signup', async function(req, res, next) {
  try {
    var useremail = req.body.useremail;
    var username = req.body.username;
    var password = req.body.password;
    var nickname = req.body.nickname;

    // 입력값 검증
    if (!useremail ||!username || !password || !nickname) {
      return res.status(400).send("모든 필드를 입력하세요.");
    }

    // 이메일 형식 검증
    if (!isValidEmail(useremail)) {
      return res.status(400).send("유효한 이메일 형식의 아이디를 입력하세요.");
    }

    // 사용자 중복 체크
    var database = req.app.get('database');
    var users = database.collection('users');

    const existingUser = await users.findOne({ useremail: useremail });
    if (existingUser) {
      return res.status(409).send("이미 존재하는 사용자입니다.");
    }

    // 비밀번호 암호화
    var salt = bcrypt.genSaltSync(saltrounds);
    var hash = bcrypt.hashSync(password, salt);

    // 신규 유저를 DB에 저장
    await users.insertOne({
      useremail: useremail,
      username: username,
      password: hash, // 해시된 비밀번호 저장
      nickname: nickname,
      rank: 18, // 초기 급수 (18급)
      rankPoints: 0, // 초기 승급 포인트
      winCount: 0, // 승리 횟수
      loseCount: 0, // 패배 횟수
      streak: 0 // 연승/연패 카운트
    });
    res.status(201).send("사용자가 성공적으로 생성되었습니다.");
  } catch (err) {
    console.error("사용자 추가 중 오류 발생: " + err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

// 로그인
router.post('/signin', async function(req, res, next) {
  try {
    var useremail = req.body.useremail;
    var username = req.body.username;
    var password = req.body.password;

    var database = req.app.get('database');
    var users = database.collection('users');

    // 입력값 검증
    if (!useremail ||!username || !password) {
      return res.status(400).send("모든 필드를 입력해주세요.");
    }

    const existingUser = await users.findOne({ useremail: useremail });
    if (existingUser) {
      var compareResult = bcrypt.compareSync(password, existingUser.password);
      if (compareResult) {
        req.session.isAuthenticated = true;
        req.session.userId = existingUser._id.toString();
        req.session.useremail = existingUser.useremail;
        req.session.username = existingUser.username;
        req.session.nickname = existingUser.nickname;
        res.json({ result: ResponseType.SUCCESS });
      } else {
        res.json({ result: ResponseType.INVALID_PASSWORD });
      }
    } else {
      res.json({ result: ResponseType.INVALID_USEREMAIL });
    }
  } catch (err) {
    console.error("로그인 중 오류 발생.", err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

// 로그아웃
router.post('/signout', function(req, res, next) {
  req.session.destroy((err) => {
    if (err) {
      console.error("로그아웃 중 오오류 발생", err);
      return res.status(500).send("서버 오류가 발생했습니다.");
    }
    res.status(200).send("로그아웃 되었습니다.");
  });
});

// 사용자 정보 요청 API
router.get('/userinfo', async function(req, res, next) {
  if (!req.session.isAutenticated) {
    return res.status(401).send("로그인이 필요합니다.");
  }

  var database = req.app.get('database');
  var users = database.collection('users');

  const user = await users.findOne({ _id: new ObjectId(req.session.userId) });
  if (!user) {
    return res.status(404).send("유저저를 찾을 수 없습니다.");
  }

  res.json({
    username: user.username,
    rank: user.rank
  });
});

// 매칭 시스템 (비슷한 급수 매칭)
router.post('/matchmaking', async function(req, res, next) {
  try {
    var { username } = req.body;
    var database = req.app.get('database');
    var users = database.collection('users');

    const user = await users.findOne({ username });
    if (!user) return res.status(404).send("유저를 찾을 수 없습니다.");

    const minRank = Math.max(1, user.rank - 1);
    const maxRank = Math.min(18, user.rank + 1);

    const possibleOpponents = await users.find({
      rank: { $gte: minRank, $lte: maxRank },
      username: { $ne: username }
    }).toArray();

    let selectedOpponent = null;
    if (possibleOpponents.length > 0) {
      selectedOpponent = possibleOpponents[Math.floor(Math.random() * possibleOpponents.length)];
    }

    res.json({
      opponents: selectedOpponent ? selectedOpponent.username: "AI",
      opponentRank: selectedOpponent ? selectedOpponent.rank: user.rank
    });
  } catch (err) {
    console.error("매칭 시스템 오류: ", err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

// 경기 결과 업데이트 (급수 시스템 반영)
router.post('/update-match-result', async function(req, res, next) {
  try {
    var { username, result } = req.body;
    var database = req.app.get('database');
    var users = database.collection('users');

    const user = await users.findOne({ username });
    if (!user) return res.status(400).send("유저를 찾을 수 없습니다.");

    if (result == "win") {
      user.rankPoints += 1;
      users.winCount += 1;
      let requiredPoints = rankSystem.find(r => user.rank >= r.min && user.rank <= r.max).pointsToRankUp;

      if (user.rankPoints >= requiredPoints && user.rank > 1) {
        user.rank -= 1;
        user.rankPoints = 0;
      }
    } else if (result == "lose") {
      user.rankPoints -= 1;
      user.loseCount += 1;
      if (user.rankPoints <= -3 && user.rank < 18) {
        user.rank += 1;
        user.rankPoints = 0;
      }
    }

    // 업데이트
    await users.updateOne({ username }, {
      $set: { rank: user.rank, rankPoints: user.rankPoints },
      $inc: { winCount: result == "win" ? 1 : 0, loseCount: result == "lose" ? 1 : 0 }
    });
    res.status(200).send("경기 결과 업데이트 완료");
  } catch (err) {
    console.error("경기 결과 업데이트 오류: " + err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

// 랭킹 시스템 (급수 및 승률 순 정렬)
router.get('/ranking', async function(req, res, next) {
  try {
    var database = req.app.get('database');
    var users = database.collection('users');

    const ranking = await users.find().sort( {rank: 1 }).toArray();
    ranking.sort((a, b) => {
      const winRateA = a.winCount / ((a.winCount + a.loseCount) || 1);
      const winRateB = b.winCount / ((b.winCount + b.loseCount) || 1);
      return winRateB - winRateA;
    });

    res.json(ranking);
  } catch (err) {
    console.error("랭킹 조회 오류: ", err);
    res.status(500).send("서버 오류가 발생했습니다.");
  }
});

module.exports = router;
