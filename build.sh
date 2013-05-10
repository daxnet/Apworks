#! /bin/bash
PARAM=''
PKGFILE='packages_v2_5_4878_35266.tar.gz'
PKGDIR='packages'
URL='http://apworks.org/wp-content/uploads/fx/'
if [ "$1" == 'Debug' ]
  then
    PARAM='/property:Configuration=MonoDebug'
elif [ "$1" == 'Release' ]
  then
    PARAM='/property:Configuration=MonoRelease'
else
  printf "\n"
  printf "Apworks Command-Line Build Tool v1.0\n\n"
  printf "Usage:\n"
  printf "    build.sh Debug\n"
  printf "        Builds the Apworks with Debug configuration.\n\n"
  printf "    build.sh Release\n"
  printf "        Builds the Apworks with Release configuration.\n\n"
  exit $?
fi
if [ ! -d $PKGDIR ]
  then
    printf "\nDownloading Dependencies...\n\n"
    rm -rf $PKGFILE
    wget $URL$PKGFILE
    if [ $? -ne 0 ]
      then
        printf "Failed to get the Dependencies from network.\n"
        exit $?
    fi
    
  tar -zxvf $PKGFILE
  rm -rf $PKGFILE
fi
xbuild $PARAM

