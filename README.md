# sudoku
* 창의적 통합설계 프로젝트
* 휴지스틱으로 스도쿠를 푸는 봇을 만들자
* 형상관리 - 빌드관리 - 배포관리(?) 프로세스를 배워봅시다

## 형상관리

* 보다시피 git 으로 관리
* 무엇이 더 必要韓紙?

## 빌드관리

* jenkins 사용
* http://58.121.74.195:8081 을 통해 젠킨스 관리자 웹 콘솔에 접근가능

### unit test tool

* 잘 모르겠음

## 배포관리

* IIS로 올린 웹 페이지를 통한 배포
* Jenkins에서 post-build command를 이용하여 원격 저장소에 게시
* http://58.121.74.195/Deploy/Sudoku.exe

# Getting started

## install git
* 윈도우 환경에서는 git for windows와 CUI 환경이 싫다면 tortoisegit을 까는 것을 추천함
* 다른 환경은 나도 몰라
* ssh를 이용한 authencication이 필요하다면 ssh 클라이언트는 OpenSSH를 사용하는것을 추천함

## git checkout
* 원격 주소 :
** https 를 사용할 경우 : https://github.com/MappyTheKat/sudoku.git
** SSH를 사용할 경우 : git@github.com:MappyTheKat/sudoku.git

# References
## 스도쿠 관련
* https://kjell.haxx.se/sudoku/  
* http://www.sudokuwiki.org/Getting_Started
* http://sudopedia.enjoysudoku.com/
* http://zhangroup.aporc.org/images/files/Paper_3485.pdf

## Jenkins-github 연동 관련
* http://yookeun.github.io/tools/2014/09/18/jenkins-github/

## VS Test Instruction
* https://msdn.microsoft.com/ko-kr/library/ms182486.aspx

## Valgrind
* http://valgrind.org/docs/manual/QuickStart.html